namespace Dreambuild.Mvc
{
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// Extends HtmlHelper to launch the fluent call chain.
    /// </summary>
    public static class HtmlHelperExtensions
    {
        private static readonly string Key = typeof(ControlFactory).AssemblyQualifiedName;

        /// <summary>
        /// Gets a ControlFactory
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns>
        public static ControlFactory Dreambuild(this HtmlHelper helper)
        {
            HttpContextBase httpContext = helper.ViewContext.HttpContext;
            ControlFactory factory = httpContext.Items[Key] as ControlFactory;

            if (factory == null)
            {
                factory = new ControlFactory(helper);
                httpContext.Items[Key] = factory;
            }

            return factory;
        }

        /// <summary>
        /// Gets a ControlFactory(Of TModel)
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="helper"></param>
        /// <returns></returns>
        public static ControlFactory<TModel> Dreambuild<TModel>(this HtmlHelper<TModel> helper) // added by WangYang@20140715
        {
            HttpContextBase httpContext = helper.ViewContext.HttpContext;
            ControlFactory<TModel> factory = httpContext.Items[Key] as ControlFactory<TModel>;

            if (factory == null)
            {
                factory = new ControlFactory<TModel>(helper);
                httpContext.Items[Key] = factory;
            }

            return factory;
        }
    }
}