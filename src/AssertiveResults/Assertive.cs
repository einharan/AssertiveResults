using System;
using System.Collections.Generic;
using AssertiveResults.Assertions;
using AssertiveResults.Contracts;
using AssertiveResults.Errors;

namespace AssertiveResults
{
    public class Assertive : IResult
    {
        protected internal List<Error> errors;
        protected internal Dictionary<string, object> metadata;

        protected Assertive()
        {
            errors = new List<Error>();
            metadata = new Dictionary<string, object>();
        }

        public bool Success => errors.Count == 0;
        public bool Failed => !Success;

        public IReadOnlyCollection<Error> Errors => errors.AsReadOnly();
        public Error FirstError => GetError(index: 0);
        public Error LastError => GetError(index: errors.Count - 1);
        public bool HasError => errors.Count > 0;

        public bool HasMetadata => metadata.Count > 0;
        public IReadOnlyDictionary<string, object> Metadata => metadata;

        public static IBegin Result()
        {
            return new Assertive();
        }

        public static IBegin<T> Result<T>()
        {
            return new Assertive<T>();
        }

        public ISubject Assert(Action<IContext> context)
        {
            if(HasError)
                return this;

            var ctx = new Context();
            context?.Invoke(ctx);

            if(ctx.Failed)
                errors.AddRange(ctx.Errors);

            return this;
        }

        public ISubject Overload()
        {
            return this;
        }

        public ISubject<T> Override<T>()
        {
            return new Assertive<T>(this);
        }

        public IResult Resolve()
        {
            return this;
        }

        public IResult Resolve(Action<IResolve> context)
        {
            if(!HasError)
                context?.Invoke(this);

            return this;
        }

        public IResult Resolve(ResolveBehavior resolveBehavior, Action<IResolve> context)
        {
            switch(resolveBehavior)
            {
                case ResolveBehavior.Control:
                    context?.Invoke(this);
                    break;
                default:
                {
                    if (!HasError)
                        break;

                    context?.Invoke(this);
                    break;
                }
            }

            return this;
        }

        public IResult WithMetadata(string metadataName, object metadataValue)
        {
            if(metadata.ContainsKey(metadataName))
                return this;

            metadata.Add(metadataName, metadataValue);
            return this;
        }

        public object GetMetadata(string metadataName)
        {
            metadata.TryGetValue(metadataName, out object value);
            return value;
        }

        private Error GetError(int index)
        {
            if(!HasError)
                throw new InvalidOperationException();

            return errors[index];
        }

        public int PurgeErrors()
        {
            int count = errors.Count;
            errors.Clear();
            return count;
        }
    }

    internal class Assertive<T> : Assertive, IResult<T>
    {
        internal Assertive()
        {
            Value = default;
        }

        internal Assertive(Assertive assertive)
        {
            this.errors = assertive.errors;
            this.metadata = assertive.metadata;
            Value = default;
        }

        public T Value { get; internal set; }

        public new ISubject<T> Assert(Action<IContext> context)
        {
           if(HasError)
                return this;

            var ctx = new Context();
            context?.Invoke(ctx);
            if(ctx.Failed)
                errors.AddRange(ctx.Errors);

            return this;
        }

        public new ISubject<T> Overload()
        {
            return this;
        }

        public new ISubject<U> Override<U>()
        {
            return new Assertive<U>(this);
        }

        public ISubject<U> Override<U>(out T value)
        {
            value = this.Value;
            return new Assertive<U>(this);
        }

        public ISubject Override()
        {
            return this;
        }

        public IResult<T> Resolve(Func<IResolve, T> context)
        {
            Value = HasError ? default : context(this);
            return this;
        }

        public IResult<T> Resolve(ResolveBehavior resolveBehavior, Func<IResolve, T> context)
        {
            switch(resolveBehavior)
            {
                case ResolveBehavior.Control:
                    return Result();
                default:
                {
                    if(HasError)
                        return this;

                    return Result();
                }
            }

            IResult<T> Result()
            {
                Value = context(this);
                return this;
            }
        }

        public void Match(Action<T> onValue, Action<IProblem> onError)
        {
            if(HasError)
                onError(this);
            else
                onValue(Value);
        }

        public U Match<U>(Func<T, U> onValue, Func<IProblem, U> onError)
        {
            if(HasError)
                return onError(this);

            return onValue(Value);
        }

        public new IResult<T> WithMetadata(string metadataName, object metadataValue)
        {
            if(metadata.ContainsKey(metadataName))
                return this;

            metadata.Add(metadataName, metadataValue);
            return this;
        }
    }
}