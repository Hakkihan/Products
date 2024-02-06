using Microsoft.Extensions.Logging;
using Moq;
using Products.Controllers;
using Products.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace ProductsTest
{
    internal class ProductsControllerTests
    {
        private ProductsController _productsController;

        [SetUp]
        public void Setup()
        {
            // Mock ILogger
            var logger = new Mock<ILogger<ProductsController>>();

            // Initialize ProductsController with the mocked ILogger
            _productsController = new ProductsController(logger.Object);
        }

        [Test]
        public void HealthCheck_Returns_OK()
        {
            // Act
            var result = _productsController.HealthCheck();

            // Assert
            Assert.That(result, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public void GetProducts_Returns_ProductList()
        {           
            // Act
            var result = _productsController.GetProducts();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IList<Product>>(result);
            CollectionAssert.AreEquivalent(ProductsController.ProductsList, result);
        }

        [TestCase("Red")]
        [TestCase("RED")]
        public void GetAllProductsWithColour_Returns_ProductsOfSpecificColour(string colour)
        {
            // Act
            var result = _productsController.GetAllProductsWithColour(colour);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IList<Product>>(result);
            CollectionAssert.AreEquivalent(
                ProductsController.ProductsList.Where(p => p.Colour.ToUpperInvariant() == colour.ToUpperInvariant()).ToList(),
                result
            );
        }

        [TestCase("NonExistentColour")]
        public void GetAllProductsWithColour_Returns_EmptyListForNonExistentColour(string colour)
        {
            // Act
            var result = _productsController.GetAllProductsWithColour(colour);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IList<Product>>(result);
            CollectionAssert.IsEmpty(result);
        }
    }
}
