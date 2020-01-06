using UnityEngine;
using System.Collections;

/// <summary>
/// An item which can be chained with other IChainableSeries objects
/// </summary>
public interface IChainableSeries<T>
{
    T GetData();
    bool GetCanHaveChildren();
    bool GetCanHaveParents();
    IChainableSeries<T> GetChild();
    IChainableSeries<T> GetParent();
    /// <summary>
    /// Set the parent of this chainable. Necessary when restablishing the chain after a child has been spliced in
    ///   pass in null to clip self out of the current chain. Be careful - this will not set the accompanying parent's child to null
    ///     Calling into this method will not also trigger OnSelfEjected
    /// </summary>
    void SetParent(IChainableSeries<T> parent);

    /// <summary>
    /// Attempts to clear the child of this parent. Will only clear if the child passed in is the same child registered on the parent
    /// </summary>
    /// <param name="child">The child to abort</param>
    /// <returns>true if a child was removed</returns>
    bool AbortChild(IChainableSeries<T> child);

    /// <summary>
    /// Called whenever this chainable has been ejected from its chain as a result of a modification to the chain
    ///  This only happens when the -parent- of this chainable is removed. Not called when its children are removed or modified
    ///  Mostly used as just an event callback to allow the derived class to respond to the ejection if necessary
    /// </summary>
    void OnSelfEjected();

    /// <summary>
    /// Attempt to splice a child and any children under that child into this series.
    /// </summary>
    /// <param name="chainHead">The head of the child to be attached</param>
    /// <returns>true if the child has been spliced in, false otherwise</returns>
    void SpliceChildIn(IChainableSeries<T> chainHead);

    /// <summary>
    /// Attempt to simply append a child to this chain. Will only work if this component accepts children, and has no current child
    ///     Will throw error if this item cannot have children, or if it currently has a child
    ///     Will not trigger any update cycles
    /// </summary>
    /// <param name="child">The head of the child to be attached</param>
    void SimpleAppendChild(IChainableSeries<T> child);

    void OnChildUpdated(IChainableSeries<T> child);
    void OnParentUpdated(Vector2 childTranslation);

}

public class ChainableSeriesUtilities
{
    /// <summary>
    /// Iterate through the children of origin until an entry with no child is found
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="origin"></param>
    /// <returns></returns>
    public static IChainableSeries<T> GetChainTerminator<T>(IChainableSeries<T> origin)
    {
        var currentTerminator = origin;
        while (currentTerminator.GetChild() != null)
        {
            currentTerminator = currentTerminator.GetChild();
        }
        return currentTerminator;
    }

    /// <summary>
    /// after a child has been replaced by a new chain spliced in place of it, this method will attempt to append the original child
    ///     at the end of the chain which was spliced in. If that's not possible, then the original child is ejected
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="originalChild">The child which has been displaced from its parent</param>
    /// <param name="newChild">The head of the chainable chain which took the place of the original child</param>
    /// <returns></returns>
    public static void UpdateOriginalChildAfterSplice<T>(IChainableSeries<T> originalChild, IChainableSeries<T> newChild)
    {
        if (originalChild != null)
        {
            originalChild.SetParent(null);

            var newTerminator = GetChainTerminator(newChild);

            if (newTerminator.GetCanHaveChildren())
            {
                //May not need to go full recursive here -- at this point, newTerminator has no children and originaChild has no parents
                // Could end up being a simple linking method??
                newTerminator.SimpleAppendChild(originalChild);
            }
            else
            {
                //the new chain has a no-append terminator. Kick out the old chain after to replacing it with the new
                originalChild.OnSelfEjected();
            }
        }
    }
}
