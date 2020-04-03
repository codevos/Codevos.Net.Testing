using Xunit;

namespace Codevos.Net.Testing
{
    /// <summary>
    /// Feature test base class.
    /// </summary>
    /// <typeparam name="TTestWebHostFactory">The <see cref="TestWebHostFactory{TStartup}"/> derived type.</typeparam>
    /// <typeparam name="TStartup">The startup class type.</typeparam>
    public abstract class FeatureTestBase<TTestWebHostFactory, TStartup> : IClassFixture<TTestWebHostFactory>
        where TTestWebHostFactory : TestWebHostFactory<TStartup>
        where TStartup : class
    {
        /// <summary>
        /// Gets the test webhost factory.
        /// </summary>
        protected readonly TTestWebHostFactory TestWebHostFactory;

        private TestWebHost<TStartup> testWebHost;
        /// <summary>
        /// Gets the default test webhost to run tests against.
        /// </summary>
        protected TestWebHost<TStartup> TestWebHost => testWebHost ?? (testWebHost = TestWebHostFactory.Create("__default__"));

        /// <summary>
        /// Initializes a new intance of the <see cref="FeatureTestBase{TTestWebHostFactory, TStartup}"/> class.
        /// </summary>
        /// <param name="testWebHostFactory">The test webhost factory to create test webshost instances with.</param>
        public FeatureTestBase(TTestWebHostFactory testWebHostFactory)
        {
            TestWebHostFactory = testWebHostFactory;
        }
    }
}