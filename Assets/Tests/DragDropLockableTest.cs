using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Tests
{
    public class DragDropLockableTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void WhenDraggedUpdatesPosition()
        {
            var gameObject = new GameObject();
            var dragger = gameObject.AddComponent<DragDropLockable>();
            gameObject.AddComponent<Image>();
            dragger.Start();
            dragger.transform.position = new Vector3(0, 0, 0);

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
            var gameObjectParent = new GameObject("parent", new System.Type[] { typeof(RectTransform) });
            gameObjectParent.AddComponent<Image>();
            var parent = gameObjectParent.AddComponent<DragDropLockable>();
            var parentRectTransform = gameObjectParent.GetComponent<RectTransform>();
            parent.transform.position = new Vector3(3, 3, 0);
            parentRectTransform.sizeDelta = new Vector2(5, 5);
            parent.Start();

            var gameObjectChild = new GameObject("child");
            gameObjectChild.AddComponent<Image>();
            var child = gameObjectChild.AddComponent<DragDropLockable>();
            child.transform.position = new Vector3(10, 10, 0);
            child.Start();

            child.OnBeginDrag(null);
            child.OnDrag(new UnityEngine.EventSystems.PointerEventData(null)
            {
                delta = new Vector2(-5, -5)
            });
            parent.OnPointerEnter(null);

            child.OnEndDrag(null);

            Assert.AreEqual(3, child.transform.position.x);
            Assert.AreEqual(-2, child.transform.position.y);


            // Use the Assert class to test conditions
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
