using System;

namespace Omnifactotum;

/// <summary>
///     Represents the relation of the owner node to the items contained in the collection.
/// </summary>
[Serializable]
internal enum DirectedGraphOwnerRelation
{
    /// <summary>
    ///     The owner is the head node.
    /// </summary>
    Head,

    /// <summary>
    ///     The owner is the tail node.
    /// </summary>
    Tail
}