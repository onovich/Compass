using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MortiseFrame.Compass {

    public class PriorityQueue<T> : IEnumerable<T> {
        List<Tuple<T, float>> elements = new List<Tuple<T, float>>();
        public List<Tuple<T, float>> Elements => elements;
        Dictionary<T, int> elementIndices = new Dictionary<T, int>();

        public int Count {
            get { return elements.Count; }
        }

        public void Enqueue(T item, float priority) {
            elements.Add(Tuple.Create(item, priority));
            elementIndices[item] = elements.Count - 1;
            BubbleUp(elements.Count - 1);
        }

        public T Dequeue() {
            var frontItem = elements[0].Item1;
            Swap(0, elements.Count - 1);
            elements.RemoveAt(elements.Count - 1);
            elementIndices.Remove(frontItem);
            BubbleDown(0);
            return frontItem;
        }

        public bool Contains(T item) {
            return elementIndices.ContainsKey(item);
        }

        public void UpdatePriority(T item, float newPriority) {
            int index = elementIndices[item];
            float oldPriority = elements[index].Item2;
            elements[index] = Tuple.Create(item, newPriority);
            if (newPriority < oldPriority) {
                BubbleUp(index);
            } else {
                BubbleDown(index);
            }
        }

        private void BubbleUp(int index) {
            while (index > 0) {
                int parentIndex = (index - 1) / 2;
                if (elements[index].Item2 < elements[parentIndex].Item2) {
                    Swap(index, parentIndex);
                    index = parentIndex;
                } else {
                    break;
                }
            }
        }

        private void BubbleDown(int index) {
            while (true) {
                int childIndex = 2 * index + 1;
                if (childIndex >= elements.Count) {
                    break;
                }
                if (childIndex + 1 < elements.Count && elements[childIndex + 1].Item2 < elements[childIndex].Item2) {
                    childIndex++;
                }
                if (elements[childIndex].Item2 < elements[index].Item2) {
                    Swap(index, childIndex);
                    index = childIndex;
                } else {
                    break;
                }
            }
        }

        private void Swap(int indexA, int indexB) {
            var temp = elements[indexA];
            elements[indexA] = elements[indexB];
            elements[indexB] = temp;

            elementIndices[elements[indexA].Item1] = indexA;
            elementIndices[elements[indexB].Item1] = indexB;
        }

        public void Clear() {
            elements.Clear();
            elementIndices.Clear();
        }

        public IEnumerator<T> GetEnumerator() {
            foreach (var element in elements) {
                yield return element.Item1;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

    }

}