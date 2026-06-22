using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WineShop.Domain.DomainModels;
using WineShop.Domain.DTO;
using WineShop.Repository.Interface;
using WineShop.Services.Interface;

namespace WineShop.Services.Implementation
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IRepository<ProductInShoppingCart> _productInShoppingCartRepository;
        private readonly IRepository<ProductAward> _productAwardRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(
     IProductRepository productRepository,
     IUserRepository userRepository,
     ILogger<ProductService> logger,
     IRepository<ProductInShoppingCart> productInShoppingCartRepository,
     IRepository<ProductAward> productAwardRepository)
        {
            _productRepository = productRepository;
            _userRepository = userRepository;
            _productInShoppingCartRepository = productInShoppingCartRepository;
            _productAwardRepository = productAwardRepository;
            _logger = logger;
        }

        //public bool AddToShoppingCart(AddToShoppingCartDto item, string userID)
        //{
        //    var user = this._userRepository.Get(userID);
        //    var userShoppingCard = user.UserCart;

        //    if (item.ProductId != null && userShoppingCard != null)
        //    {
        //        var product = this.GetDetailsForProduct(item.ProductId);
        //        if (product != null)
        //        {
        //            ProductInShoppingCart itemToAdd = new ProductInShoppingCart
        //            {
        //                Product = product,
        //                ProductId = product.Id,
        //                ShoppingCart = userShoppingCard,
        //                ShoppingCartId = userShoppingCard.Id,
        //                Quantity = item.Quantity,
        //            };

        //            this._productInShoppingCartRepository.Insert(itemToAdd);
        //            _logger.LogInformation("Product was successfully added into ShoppingCart!");

        //            return true;
        //        }
        //        return false;

        //    }
        //    _logger.LogInformation("Something was wrong. ProductId or UseShoppingCard may be unavailable!");

        //    return false;


        //}
        public bool AddToShoppingCart(AddToShoppingCartDto item, string userID)
        {
            var user = this._userRepository.Get(userID);
            var userShoppingCart = user.UserCart;

            if (item.ProductId != null && userShoppingCart != null)
            {
                var product = this.GetDetailsForProduct(item.ProductId);
                if (product == null)
                {
                    _logger.LogInformation("Product not found.");
                    return false;
                }

                var existingItem = userShoppingCart.ProductInShoppingCarts
                    .FirstOrDefault(x => x.ProductId == product.Id);

                if (existingItem != null)
                {
                    existingItem.Quantity += item.Quantity;
                    this._productInShoppingCartRepository.Update(existingItem);
                    _logger.LogInformation("Product quantity updated in ShoppingCart.");
                }
                else
                {
                    ProductInShoppingCart itemToAdd = new ProductInShoppingCart
                    {
                        Id = Guid.NewGuid(),
                        Product = product,
                        ProductId = product.Id,
                        ShoppingCart = userShoppingCart,
                        ShoppingCartId = userShoppingCart.Id,
                        Quantity = item.Quantity,
                    };

                    this._productInShoppingCartRepository.Insert(itemToAdd);
                    _logger.LogInformation("Product was successfully added into ShoppingCart.");
                }

                return true;
            }

            _logger.LogInformation("Something was wrong. ProductId or UserShoppingCart may be unavailable.");
            return false;
        }

        public void CreateNewProduct(Product p)
        {
            this._productRepository.Insert(p);
        }

        public void DeleteProduct(Guid id)
        {
            var product = this.GetDetailsForProduct(id);
            this._productRepository.Delete(product);
        }

        public List<Product> GetAllProducts()
        {
            _logger.LogInformation("GetAllProducts was called!");
            return this._productRepository.GetAll().ToList();
        }

        public Product GetDetailsForProduct(Guid? id)
        {
            return this._productRepository.GetProductWithAwards(id);
        }

        public AddToShoppingCartDto GetShoppingCartInfo(Guid? id)
        {
            var product = this.GetDetailsForProduct(id);
            AddToShoppingCartDto model = new AddToShoppingCartDto
            {
                SelectedProduct = product,
                ProductId = product.Id,
                Quantity = 1
            };

            return model;
        }


        public void UpdeteExistingProduct(Product p)
        {
            this._productRepository.Update(p);
        }
        public void AddAwardToProduct(AddProductAwardDto model)
        {
            var product = _productRepository.Get(model.ProductId);

            if (product == null)
                throw new Exception("Product not found.");

            var award = new ProductAward
            {
                Id = Guid.NewGuid(),
                ProductId = model.ProductId,
                CompetitionName = model.CompetitionName,
                AwardName = model.AwardName,
                AwardYear = model.AwardYear
            };

            _productAwardRepository.Insert(award);
        }

        public void DeleteAward(Guid awardId)
        {
            var award = _productAwardRepository.Get(awardId);

            if (award == null)
                throw new Exception("Award not found.");

            _productAwardRepository.Delete(award);
        }

        public List<Product> FilterProducts(ProductFilterDto filter)
        {
            var query = this._productRepository.GetAllWithAwards().AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.ToLower();

                query = query.Where(x =>
                    x.ProductName.ToLower().Contains(term) ||
                    x.ProductDescription.ToLower().Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(filter.Color))
            {
                query = query.Where(x => x.Color == filter.Color);
            }

            if (!string.IsNullOrWhiteSpace(filter.AwardName))
            {
                query = query.Where(x => x.ProductAwards.Any(a => a.AwardName == filter.AwardName));
            }

            if (filter.OnlyAvailable)
            {
                query = query.Where(x => x.Quantity > 0);
            }

            if (filter.PriceSort == "priceAsc")
            {
                query = query.OrderBy(x => x.Price);
            }
            else if (filter.PriceSort == "priceDesc")
            {
                query = query.OrderByDescending(x => x.Price);
            }
            else if (filter.YearSort == "yearDesc")
            {
                query = query.OrderByDescending(x => x.Year);
            }
            else if (filter.YearSort == "yearAsc")
            {
                query = query.OrderBy(x => x.Year);
            }
            else
            {
                query = query.OrderBy(x => x.ProductName);
            }

            return query.ToList();
        }

    }
}