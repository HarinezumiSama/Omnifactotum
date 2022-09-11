using Omnifactotum.Annotations;

namespace Omnifactotum
{
    /// <summary>
    ///     Encapsulates a method that has an <see langword="out"/> parameter and that returns a value
    ///     of the type specified by the <typeparamref name="TResult"/> parameter.
    /// </summary>
    /// <typeparam name="TOutput">
    ///     The type of the <see langword="out"/> parameter of the method that this delegate encapsulates.
    /// </typeparam>
    /// <typeparam name="TResult">
    ///     The type of the return value of the method that this delegate encapsulates. This type parameter is covariant.
    /// </typeparam>
    /// <param name="outArg">
    ///     The <see langword="out"/> parameter of the method that this delegate encapsulates.
    /// </param>
    /// <returns>
    ///     The return value of the method that this delegate encapsulates.
    /// </returns>
    [PublicAPI]
    public delegate TResult OutFunc<TOutput, out TResult>(out TOutput outArg);

    /// <summary>
    ///     Encapsulates a method that has a regular parameter and an <see langword="out"/> parameter and that returns a value
    ///     of the type specified by the <typeparamref name="TResult"/> parameter.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the regular parameter of the method that this delegate encapsulates. This type parameter is contravariant.
    /// </typeparam>
    /// <typeparam name="TOutput">
    ///     The type of the <see langword="out"/> parameter of the method that this delegate encapsulates.
    /// </typeparam>
    /// <typeparam name="TResult">
    ///     The type of the return value of the method that this delegate encapsulates. This type parameter is covariant.
    /// </typeparam>
    /// <param name="arg">
    ///     The regular parameter of the method that this delegate encapsulates.
    /// </param>
    /// <param name="outArg">
    ///     The <see langword="out"/> parameter of the method that this delegate encapsulates.
    /// </param>
    /// <returns>
    ///     The return value of the method that this delegate encapsulates.
    /// </returns>
    [PublicAPI]
    public delegate TResult OutFunc<in T, TOutput, out TResult>(T arg, out TOutput outArg);

    /// <summary>
    ///     Encapsulates a method that has two regular parameters and an <see langword="out"/> parameter and that returns a value
    ///     of the type specified by the <typeparamref name="TResult"/> parameter.
    /// </summary>
    /// <typeparam name="T1">
    ///     The type of the first regular parameter of the method that this delegate encapsulates. This type parameter is contravariant.
    /// </typeparam>
    /// <typeparam name="T2">
    ///     The type of the second regular parameter of the method that this delegate encapsulates. This type parameter is contravariant.
    /// </typeparam>
    /// <typeparam name="TOutput">
    ///     The type of the <see langword="out"/> parameter of the method that this delegate encapsulates.
    /// </typeparam>
    /// <typeparam name="TResult">
    ///     The type of the return value of the method that this delegate encapsulates. This type parameter is covariant.
    /// </typeparam>
    /// <param name="arg1">
    ///     The first regular parameter of the method that this delegate encapsulates.
    /// </param>
    /// <param name="arg2">
    ///     The second regular parameter of the method that this delegate encapsulates.
    /// </param>
    /// <param name="outArg">
    ///     The <see langword="out"/> parameter of the method that this delegate encapsulates.
    /// </param>
    /// <returns>
    ///     The return value of the method that this delegate encapsulates.
    /// </returns>
    [PublicAPI]
    public delegate TResult OutFunc<in T1, in T2, TOutput, out TResult>(T1 arg1, T2 arg2, out TOutput outArg);

    /// <summary>
    ///     Encapsulates a method that has three regular parameters and an <see langword="out"/> parameter and that returns a value
    ///     of the type specified by the <typeparamref name="TResult"/> parameter.
    /// </summary>
    /// <typeparam name="T1">
    ///     The type of the first regular parameter of the method that this delegate encapsulates. This type parameter is contravariant.
    /// </typeparam>
    /// <typeparam name="T2">
    ///     The type of the second regular parameter of the method that this delegate encapsulates. This type parameter is contravariant.
    /// </typeparam>
    /// <typeparam name="T3">
    ///     The type of the third regular parameter of the method that this delegate encapsulates. This type parameter is contravariant.
    /// </typeparam>
    /// <typeparam name="TOutput">
    ///     The type of the <see langword="out"/> parameter of the method that this delegate encapsulates.
    /// </typeparam>
    /// <typeparam name="TResult">
    ///     The type of the return value of the method that this delegate encapsulates. This type parameter is covariant.
    /// </typeparam>
    /// <param name="arg1">
    ///     The first regular parameter of the method that this delegate encapsulates.
    /// </param>
    /// <param name="arg2">
    ///     The second regular parameter of the method that this delegate encapsulates.
    /// </param>
    /// <param name="arg3">
    ///     The third regular parameter of the method that this delegate encapsulates.
    /// </param>
    /// <param name="outArg">
    ///     The <see langword="out"/> parameter of the method that this delegate encapsulates.
    /// </param>
    /// <returns>
    ///     The return value of the method that this delegate encapsulates.
    /// </returns>
    [PublicAPI]
    public delegate TResult OutFunc<in T1, in T2, in T3, TOutput, out TResult>(T1 arg1, T2 arg2, T3 arg3, out TOutput outArg);
}