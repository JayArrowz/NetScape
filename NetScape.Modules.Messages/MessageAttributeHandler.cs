using Autofac;
using NetScape.Abstractions.Interfaces;
using NetScape.Abstractions.Interfaces.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace NetScape.Modules.Messages
{
    public class MessageAttributeHandler : IDisposable
    {
        private readonly List<IDisposable> _subscriptions;
        private ContainerProvider _containerProvider;
        private readonly IMessageDecoder[] _decoders;

        public MessageAttributeHandler(ContainerProvider containerProvider, IMessageDecoder[] decoders)
        {
            _containerProvider = containerProvider;
            _decoders = decoders;
            _subscriptions = new();
        }

        public void Dispose()
        {
            _subscriptions.ForEach(t => t.Dispose());
        }

        public void Start()
        {
            var methods = AppDomain.CurrentDomain.GetAssemblies()
                       .SelectMany(t => t.GetTypes())
                       .SelectMany(t => t.GetMethods())
                       .Where(t => t.GetCustomAttribute<MessageAttribute>(false) != null)
                       .ToList();
            var classes = methods.Select(t => t.DeclaringType).Distinct().ToList();
            foreach (var clazz in classes)
            {
                var resolvedClazz = _containerProvider.Container.Resolve(clazz.GetTypeInfo().AsType());
                var subMethods = methods.Where(t => t.DeclaringType.FullName == clazz.FullName);
                foreach (var subscriptionMethod in subMethods)
                {
                    var customAttribute = subscriptionMethod.GetCustomAttribute<MessageAttribute>(false);
                    var messageDecoder = _decoders.First(decoder => decoder.TypeName == customAttribute.Type.Name);

                    var parameters = subscriptionMethod.GetParameters().Select(p => Expression.Parameter(p.ParameterType, p.Name)).ToArray();
                    Expression call = Expression.Call(Expression.Constant(resolvedClazz), subscriptionMethod, parameters);
                    var isAsync = subscriptionMethod.ReturnType == typeof(Task) ? true : false;
                    var hasFilter = !string.IsNullOrEmpty(customAttribute.FilterPropertyName);
                    Type actionType = isAsync ? Expression.GetFuncType(parameters[0].Type, typeof(Task)) : typeof(Action<>).MakeGenericType(parameters[0].Type);
                    Delegate expressionDelegate = Expression.Lambda(actionType, call, parameters).Compile();
                    Delegate filterDelegate = hasFilter ? (Delegate) clazz.GetDeclaredProperty(customAttribute.FilterPropertyName).GetValue(resolvedClazz) : null;
                    if (isAsync)
                    {
                        _subscriptions.Add(messageDecoder.SubscribeDelegateAsync(expressionDelegate, filterDelegate));
                    }
                    else
                    {
                        _subscriptions.Add(messageDecoder.SubscribeDelegate(expressionDelegate, filterDelegate));
                    }
                    Serilog.Log.Logger.Debug("Subscribed to {0} - {1} for message {2}", messageDecoder.GetType().Name, subscriptionMethod, messageDecoder.TypeName);
                }
            }
        }
    }

    /// <summary>
    /// This annotation must be defined if <see cref="MessageAttribute"/> is used to enable scanning.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class MessageHandlerAttribute : Attribute
    {
        public MessageHandlerAttribute()
        {
        }
    }

    /// <summary>
    /// All classes marked with this annotation will be scanned and retrived from the DI container. The method
    /// will be a observer of the message <see cref="Type"/>. <see cref="MessageHandlerAttribute"/> must be defined 
    /// on the class if this attribute is defined.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class MessageAttribute : Attribute
    {
        /// <summary>
        /// Gets the type of the message.
        /// </summary>
        /// <value>
        /// The message type.
        /// </value>
        public Type Type { get; }

        /// <summary>
        /// Gets the name of the filter property. If this value is null
        /// then the method will not go through filtering before it is ran. The type of 
        /// property must be Predicate<T> where T is <see cref="Type"/>.
        /// </summary>
        /// <value>
        /// The name of the filter property.
        /// </value>
        public string FilterPropertyName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageAttribute"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="filterPropertyName">Name of the filter property.</param>
        public MessageAttribute(Type type, string filterPropertyName)
        {
            Type = type;
            FilterPropertyName = filterPropertyName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageAttribute"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public MessageAttribute(Type type) : this(type, null)
        {
        }
    }
}
