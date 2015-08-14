using System;
using System.Collections.Generic;

namespace FBlock.Core
{
    /// <summary>
    /// Job interface
    /// </summary>
    public interface IJob
    {
        /// <summary>
        /// Retrieve the friendly name of a job
        /// </summary>
        string Name { get; }
    }

    /// <summary>
    /// Represents a single Job, it holds components
    /// </summary>
    /// <typeparam name="TIn">Input type of the job</typeparam>
    /// <typeparam name="TOut">Output type of the job</typeparam>
    public class Job<TIn, TOut> : Component<TIn, TOut>, IJob
    {
        #region Fields
        
        protected IComponentWrapper _start;
        protected string _name;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the fiendly name of this job
        /// </summary>
        public string Name { get { return _name; } }

        #endregion

        #region CTors

        /// <summary>
        /// Instantiates a new job with the given name
        /// </summary>
        /// <param name="name">Friendly name to use, if set to null, the type name will be used</param>
        public Job(string name = null)
        {
            _name = (string.IsNullOrEmpty(name) ? GetType().Name : name);
        }

        #endregion

        #region Chaining methods

        /// <summary>
        /// Register the start component of this job
        /// </summary>
        /// <typeparam name="TComponentOut">Output type of the component</typeparam>
        /// <param name="component">Component to add</param>
        /// <returns>A wrapper to chain components</returns>
        public ComponentWrapper<TIn, TOut, TIn, TComponentOut> Start<TComponentOut>(
            Component<TIn, TComponentOut> component)
        {
            ComponentWrapper<TIn, TOut, TIn, TComponentOut> wrappedComponent = 
                new ComponentWrapper<TIn, TOut, TIn, TComponentOut>(component);

            _start = wrappedComponent;

            return wrappedComponent;
        }

        /// <summary>
        /// Register a single component for this job
        /// </summary>
        /// <param name="component">Component to register</param>
        public void StartAndEnd(Component<TIn, TOut> component)
        {
            this.Start(component);
        }

        #endregion

        /// <summary>
        /// Process this job
        /// </summary>
        /// <param name="arg">Input argument</param>
        /// <returns>Job result</returns>
        public TOut Process(TIn arg)
        {
            return this.Process(arg, new JobContext(this));
        }

        /// <summary>
        /// Process this job
        /// </summary>
        /// <param name="arg">Input argument</param>
        /// <param name="context">Context</param>
        /// <returns>Job result</returns>
        public override TOut Process(TIn arg, JobContext context)
        {
            if (_start == null)
                throw new Exception("Job start component not defined!");

            return (TOut)_start.Activate(arg, context);
        }
    }

    /// <summary>
    /// The job context will be passed in the Process of each component. It should be used
    /// to store context specific values available across all components of a job. Use it with parcimony!
    /// </summary>
    public class JobContext : Dictionary<string, object>
    {
        #region Fields

        protected IJob _job;

        #endregion

        #region Properties

        /// <summary>
        /// Retrieve the attached job for this context
        /// </summary>
        public IJob Job { get { return _job; } }

        #endregion

        #region CTor

        /// <summary>
        /// Instantiates a new job context for the given job
        /// </summary>
        /// <param name="job">Attached job</param>
        public JobContext(IJob job)
        {
            _job = job;
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Sets a value in this job context
        /// </summary>
        /// <typeparam name="T">Type of the value to insert</typeparam>
        /// <param name="key">Unique key to set</param>
        /// <param name="value">Value to set</param>
        public void Set<T>(string key, T value)
        {
            this[key] = value;
        }

        /// <summary>
        /// Retrieve a value in this job context
        /// </summary>
        /// <typeparam name="T">Type of the value to retrieve</typeparam>
        /// <param name="key">Key to look for</param>
        /// <returns>The stored value or default(T) if no one is found</returns>
        public T Get<T>(string key)
        {
            if(this.ContainsKey(key))
                return (T)this[key];

            return default(T);
        }

        #endregion
    }
}
