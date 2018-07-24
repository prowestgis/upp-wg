using Nancy;
using Nancy.Security;
using Nancy.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UPP.Security;

namespace UPP.Common
{
    public static class SecurityHooks
    {
        public static IUserIdentity LOCAL_USER = new NancyAuthUser(new AuthToken() { Sub = "local", Email = "nobody@example.com" });

        public static void AllowFromLocalHost(this INancyModule module, IUserIdentity user = null)
        {
            module.AddBeforeHookOrExecute(SecurityHooks.CheckIsLocal(user ?? LOCAL_USER), "Allow from localhost");
        }

        private static Func<NancyContext, Response> CheckIsLocal(IUserIdentity user)
        {
            return (ctx) =>
            {
                if (ctx.Request.IsLocal() && !ctx.CurrentUser.IsAuthenticated())
                {
                    ctx.CurrentUser = user;
                }

                return null;
            };
        }

        public static void EnableCORS(this INancyModule module)
        {
            module.After.AddItemToEndOfPipeline((ctx) =>
                ctx.Response
                    .WithHeader("Access-Control-Allow-Origin", "*")
                    .WithHeader("Access-Control-Allow-Methods", "POST,GET")
                    .WithHeader("Access-Control-Allow-Headers", "Accept, Origin, Content-type")
            );
        }
    }

    public static class Helpers
    {
        public static string ExtendClaim(this string claims, string claim)
        {
            var input = claims ?? String.Empty;
            var array = input.Split(' ').Where(x => !String.IsNullOrWhiteSpace(x)).ToList();
            array.Add(claim);

            return String.Join(" ", array.Distinct());
        }

        public static IEnumerable<string> FromCSV(this string str)
        {
            return str.Split(',').Select(x => x.Trim());
        }

        public static MemberInfo GetMemberInfo<TObject, TProperty>(Expression<Func<TObject, TProperty>> expression)
        {
            var member = expression.Body as MemberExpression;
            if (member != null)
            {
                return member.Member;
            }

            throw new ArgumentException("expression");
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string property, bool ascending)
        {
            return ascending ? OrderBy<T>(source, property) : OrderByDescending(source, property);
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, "OrderBy");
        }

        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, "OrderByDescending");
        }

        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, "ThenBy");
        }

        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, "ThenByDescending");
        }

        public static IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> source, string property, string methodName)
        {
            string[] props = property.Split('.');
            Type type = typeof(T);
            ParameterExpression arg = Expression.Parameter(type, "x");
            Expression expr = arg;
            foreach (string prop in props)
            {
                // use reflection (not ComponentModel) to mirror LINQ
                PropertyInfo pi = type.GetProperty(prop);
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }

            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);

            object result = typeof(Queryable).GetMethods().Single(
                    method => method.Name == methodName
                            && method.IsGenericMethodDefinition
                            && method.GetGenericArguments().Length == 2
                            && method.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(T), type)
                    .Invoke(null, new object[] { source, lambda });
            return (IOrderedQueryable<T>)result;
        }
    }
}
