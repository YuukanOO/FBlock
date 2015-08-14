namespace FBlock.Core
{
    /// <summary>
    /// Public abstract class for FBlock components
    /// </summary>
    public abstract class IComponentWrapper
    {
        /// <summary>
        /// Activate the component
        /// </summary>
        /// <param name="arg">Input argument</param>
        /// <param name="context">Current context</param>
        /// <returns>Output argument</returns>
        internal abstract object Activate(object arg, JobContext context);
    }

    /// <summary>
    /// Wrapper for job components. Exposes a friendly API to chain components with type checking
    /// </summary>
    /// <typeparam name="JobIn">Job input type</typeparam>
    /// <typeparam name="JobOut">Job output type</typeparam>
    /// <typeparam name="ComponentIn">Component input type</typeparam>
    /// <typeparam name="ComponentOut">Component output type</typeparam>
    public class ComponentWrapper<JobIn, JobOut, ComponentIn, ComponentOut> : IComponentWrapper
    {
        #region Fields
        
        protected Component<ComponentIn, ComponentOut> _component;
        protected IComponentWrapper _next;

        #endregion

        #region CTors
        
        /// <summary>
        /// Instantiates a new wrapper for the given component
        /// </summary>
        /// <param name="innerComponent">Inner component to set</param>
        public ComponentWrapper(Component<ComponentIn, ComponentOut> innerComponent)
        {
            _component = innerComponent;
        }

        #endregion

        #region Chaining
        
        /// <summary>
        /// Mark the end of this job
        /// </summary>
        /// <param name="component">Last component to execute</param>
        public void End(Component<ComponentOut, JobOut> component)
        {
            ComponentWrapper<JobIn, JobOut, ComponentOut, JobOut> wrapperComponent = 
                new ComponentWrapper<JobIn, JobOut, ComponentOut, JobOut>(component);

            _next = wrapperComponent;
        }

        /// <summary>
        /// Register a new component to execute after this one
        /// </summary>
        /// <typeparam name="TComponentOut">Output type of the component</typeparam>
        /// <param name="component">Component to add</param>
        /// <returns>A wrapper to chain components</returns>
        public ComponentWrapper<JobIn, JobOut, ComponentOut, TComponentOut> Then<TComponentOut>(
            Component<ComponentOut, TComponentOut> component)
        {
            ComponentWrapper<JobIn, JobOut, ComponentOut, TComponentOut> wrappedComponent = 
                new ComponentWrapper<JobIn, JobOut, ComponentOut, TComponentOut>(component);

            _next = wrappedComponent;

            return wrappedComponent;
        }

        #endregion

        /// <summary>
        /// Activates this component, processes it and run the next component if any
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        internal override object Activate(object arg, JobContext context)
        {
            ComponentOut result = _component.Process((ComponentIn)arg, context);

            if (_next != null)
                return _next.Activate(result, context);

            return result;
        }
    }

    /// <summary>
    /// Represent a single component
    /// </summary>
    /// <typeparam name="TIn">Input type</typeparam>
    /// <typeparam name="TOut">Output type</typeparam>
    public abstract class Component<TIn, TOut>
    {
        /// <summary>
        /// Process this component, this is where you should write your implementation.
        /// </summary>
        /// <param name="arg">Input argument</param>
        /// <param name="context">Current job context</param>
        /// <returns>Output result</returns>
        public abstract TOut Process(TIn arg, JobContext context);
    }
}
