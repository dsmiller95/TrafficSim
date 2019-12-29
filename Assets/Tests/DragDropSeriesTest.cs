using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.ScriptableBuilder.ScriptLinking;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Tests
{
    public class TestDraggableBehavior : ScriptableEntry
    {
        public bool canChild, canParent;

        public override IScriptableEntry Execute(ICarActionable reciever)
        {
            return null;
        }

        public override bool GetCanHaveChildren()
        {
            return this.canChild;
        }

        public override bool GetCanHaveParents()
        {
            return this.canParent;
        }

        public override bool GetCompatabilityWithDraggable(BaseDragDrop draggable)
        {
            return true;
        }
    }

    public class DragDropSeriesTest
    {
        private SeriesDragDrop GetDraggable(
            float positionX = 0,
            float positionY = 0,
            float width = 0,
            float height = 0,
            string name = "draggable",
            SeriesDragDrop parent = null,
            bool canHaveChildren = true,
            bool canHaveParents = true)
        {
            var gameObject = new GameObject(name, new System.Type[] { typeof(RectTransform) });
            var dragger = gameObject.AddComponent<SeriesDragDrop>();
            gameObject.AddComponent<Image>();
            dragger.transform.position = new Vector3(positionX, positionY, 0);
            var testBehavior = gameObject.AddComponent<TestDraggableBehavior>();
            testBehavior.canChild = canHaveChildren;
            testBehavior.canParent = canHaveParents;

            dragger.Start();

            var rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(width, height);

            if (parent)
            {
                dragger.parent = parent;
                parent.nextExecutingChild = dragger;
            }

            return dragger;
        }

        // A Test behaves as an ordinary method
        [Test]
        public void WhenDraggedUpdatesPosition()
        {
            var dragger = this.GetDraggable();

            dragger.OnBeginDrag(null);
            dragger.OnDrag(new UnityEngine.EventSystems.PointerEventData(null)
            {
                delta = new Vector2(5, -5)
            });

            Assert.AreEqual(5, dragger.transform.position.x);
            Assert.AreEqual(-5, dragger.transform.position.y);

            dragger.OnEndDrag(null);

            Assert.AreEqual(5, dragger.transform.position.x);
            Assert.AreEqual(-5, dragger.transform.position.y);


            // Use the Assert class to test conditions
        }
        // A Test behaves as an ordinary method
        [Test]
        public void WhenSingleDraggedOnSingleShouldLock()
        {
            var parent = this.GetDraggable(3, 3, 5, 5);
            var child = this.GetDraggable(10, 10, 0, 0);

            child.OnBeginDrag(null);
            child.OnDrag(new UnityEngine.EventSystems.PointerEventData(null)
            {
                delta = new Vector2(-5, -5)
            });
            parent.OnPointerEnter(null);

            child.OnEndDrag(null);

            Assert.AreEqual(3, child.transform.position.x);
            Assert.AreEqual(-2, child.transform.position.y);
            Assert.AreEqual(parent, child.parent);
            Assert.AreEqual(child, parent.nextExecutingChild);


            // Use the Assert class to test conditions
        }

        [Test]
        public void WhenDoubleLinkedDraggedOntoSingleShouldCorrectlyOrderChain()
        {
            var parent1 = this.GetDraggable();
            var parent2 = this.GetDraggable();
            var child2 = this.GetDraggable(parent: parent2);

            parent2.OnBeginDrag(null);
            parent1.OnPointerEnter(null);
            parent2.OnEndDrag(null);

            AssertChainOrder(parent1, parent2, child2);
        }

        [Test]
        public void WhenSingleLinkedDraggedOntoDoubleShouldCorrectlyOrderChain()
        {
            var parent1 = this.GetDraggable();
            var parent2 = this.GetDraggable();
            var child2 = this.GetDraggable(parent: parent2);

            parent1.OnBeginDrag(null);
            parent2.OnPointerEnter(null);
            parent1.OnEndDrag(null);

            AssertChainOrder(parent2, parent1, child2);
        }

        [Test]
        public void WhenDoubleLinkedDraggedOntoDoubleShouldCorrectlyOrderChain()
        {
            var parent1 = this.GetDraggable(name: "parent1");
            var child1 = this.GetDraggable(name: "child1", parent: parent1);
            var parent2 = this.GetDraggable(name: "parent2");
            var child2 = this.GetDraggable(name: "child2", parent: parent2);

            parent1.OnBeginDrag(null);
            parent2.OnPointerEnter(null);
            parent1.OnEndDrag(null);

            AssertChainOrder(parent2, parent1, child1, child2);
        }

        [Test]
        public void WhenDoubleLinkedWithNoChildTerminatorDraggedOntoDoubleShouldCorrectlyOrderChain()
        {
            var parent1 = this.GetDraggable(name: "parent1");
            var child1 = this.GetDraggable(name: "child1", parent: parent1, canHaveChildren: false);
            var parent2 = this.GetDraggable(name: "parent2");
            var child2 = this.GetDraggable(0, 0, 20, 10, name: "child2", parent: parent2);

            parent1.OnBeginDrag(null);
            parent2.OnPointerEnter(null);
            parent1.OnEndDrag(null);

            AssertChainOrder(parent2, parent1, child1);
            Assert.AreEqual(child2.parent, null);
            Assert.AreEqual(child2.nextExecutingChild, null);
            Assert.AreEqual(20f, child2.rectTransform.position.x);
        }

        [Test]
        public void WhenDoubleLinkedWithNoChildTerminatorDraggedOntoDoubleWithNoChildTerminatorShouldOrphanTerminator()
        {
            var parent1 = this.GetDraggable(name: "parent1");
            var child1 = this.GetDraggable(name: "child1", parent: parent1, canHaveChildren: false);
            var parent2 = this.GetDraggable(name: "parent2");
            var child2 = this.GetDraggable(0, 0, 20, 10, name: "child2", parent: parent2, canHaveChildren: false);

            parent1.OnBeginDrag(null);
            parent2.OnPointerEnter(null);
            parent1.OnEndDrag(null);

            AssertChainOrder(parent2, parent1, child1);
            Assert.AreEqual(null, child2.parent);
            Assert.AreEqual(null, child2.nextExecutingChild);
            Assert.AreEqual(20, child2.rectTransform.position.x);
        }

        [Test]
        public void WhenTripleLinkedWithNoChildTerminatorDraggedOntoTriplWithNoChildTerminatorShouldOrphanStaticChainChildren()
        {
            var parent1 = this.GetDraggable(name: "parent1");
            var child1 = this.GetDraggable(name: "child1", parent: parent1);
            var grandchild1 = this.GetDraggable(name: "grandchild1", parent: child1, canHaveChildren: false);
            var parent2 = this.GetDraggable(name: "parent2");
            var child2 = this.GetDraggable(0, 0, 20, 10, name: "child2", parent: parent2);
            var grandchild2 = this.GetDraggable(name: "grandchild2", parent: child2, canHaveChildren: false);

            parent1.OnBeginDrag(null);
            parent2.OnPointerEnter(null);
            parent1.OnEndDrag(null);

            AssertChainOrder(parent2, parent1, child1, grandchild1);
            AssertChainOrder(child2, grandchild2);
            Assert.AreEqual(20, child2.rectTransform.position.x);
            Assert.AreEqual(20, grandchild2.rectTransform.position.x);
        }

        [Test]
        public void WhenSingleNoChildTerminatorDraggedToParentShouldCascadeToBottom()
        {
            var noChildren1 = this.GetDraggable(name: "parent1", canHaveChildren: false);
            var parent2 = this.GetDraggable(name: "parent2");
            var child2 = this.GetDraggable(name: "child2", parent: parent2);

            noChildren1.OnBeginDrag(null);
            parent2.OnPointerEnter(null);
            noChildren1.OnEndDrag(null);

            AssertChainOrder(parent2, child2, noChildren1);
        }

        [Test]
        public void WhenSingleNoChildTerminatorDraggedToChainWithTerminatorShouldCascadeToBottomAndEjectOldTerminator()
        {
            var noChildren1 = this.GetDraggable(name: "noChildren1", canHaveChildren: false);
            var parent2 = this.GetDraggable(name: "parent2");
            var child2 = this.GetDraggable(name: "child2", parent: parent2, canHaveChildren: false);

            noChildren1.OnBeginDrag(null);
            parent2.OnPointerEnter(null);
            noChildren1.OnEndDrag(null);

            AssertChainOrder(parent2, noChildren1);
            Assert.AreEqual(null, child2.parent);
            Assert.AreEqual(null, child2.nextExecutingChild);
        }

        private void AssertParentChild(SeriesDragDrop parent, SeriesDragDrop child)
        {
            Assert.AreEqual(child, parent.nextExecutingChild);
            Assert.AreEqual(parent, child.parent);
        }

        /// <summary>
        /// Expects to be in parent towards child order
        /// </summary>
        /// <param name="chain"></param>
        private void AssertChainOrder(params SeriesDragDrop[] chain) {
            Assert.AreEqual(null, chain[0].parent);
            for(var i = 0; i < chain.Length - 1; i++)
            {
                AssertParentChild(chain[i], chain[i + 1]);
            }
            Assert.AreEqual(null, chain[chain.Length - 1].nextExecutingChild);
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator TestWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
