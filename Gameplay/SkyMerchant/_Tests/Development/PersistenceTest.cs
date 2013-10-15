using System;
using MHGameWork.TheWizards.Rendering;
using NSubstitute;
using NSubstitute.Core;
using NUnit.Framework;
using SlimDX;
using MHGameWork.TheWizards.SkyMerchant._Tests.Ideas;

namespace MHGameWork.TheWizards.SkyMerchant._Tests.Development
{
    /// <summary>
    /// Test game state persistence
    /// </summary>
    public class PersistenceTest
    {
        public void TestPersistSingleComponentState()
        {
            var persister = new ObjectPersister();

            var comp = new GameObjectComponent();
            comp.SetTestData();

            var data = persister.SerializeObjectState(comp);

            var comp2 = new GameObjectComponent();

            persister.DeserializeObjectState(comp2, data);
            Assert.True(comp.Equals(comp2));
        }

        #region Test classes

        private class GameObjectComponent
        {
            private int data1;
            private int data2;
            private Vector3 vector;
            public int data3;
            public string Number;


            public GameObjectComponent()
            {

            }

            public void SetTestData()
            {
                data1 = 1;
                data2 = 2;
                vector = new Vector3(5, 8, 13);
                data3 = 1337;
                Number = "Universe";
            }

            protected bool Equals(GameObjectComponent other)
            {
                return data1 == other.data1 && data2 == other.data2 && vector.Equals(other.vector) &&
                       data3 == other.data3 && string.Equals(Number, other.Number);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((GameObjectComponent)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = data1;
                    hashCode = (hashCode * 397) ^ data2;
                    hashCode = (hashCode * 397) ^ vector.GetHashCode();
                    hashCode = (hashCode * 397) ^ data3;
                    hashCode = (hashCode * 397) ^ (Number != null ? Number.GetHashCode() : 0);
                    return hashCode;
                }
            }

            public override string ToString()
            {
                return string.Format("Data1: {0}, Data2: {1}, Vector: {2}, Data3: {3}, Number: {4}", data1, data2,
                                     vector, data3, Number);
            }
        }



        #endregion



        public void Test()
        {
            var persister = new GameObjectPersister();

            var comp = new GameObjectComponent();
            comp.SetTestData();

            var obj = createGameObject(comp);

            var data = persister.SerializeGameObject(obj);


            var obj2 = persister.DeserializeGameObject(data);

            Assert.True(obj.Equals(obj2));
        }

        private IGameObject createGameObject(GameObjectComponent comp)
        {
            var subs = Substitute.For<IGameObject>();
            subs.Set<GameObjectComponent>(comp);
            subs.Equals(null).Returns(delegate(CallInfo info)
                {
                    var other = info.Args()[0] as IGameObject;
                    var comp2 = other.Get<GameObjectComponent>();
                    return comp.Equals(comp2);
                });

            return subs;
        }

        public interface IGameObject
        {

        }

        public interface IGameObjectsFactory
        {
            object Resolve(Type t);
        }


    }

    public class GameObjectPersister
    {
        public object SerializeGameObject(PersistenceTest.IGameObject gameObject)
        {
            throw new NotImplementedException();
        }

        public object DeserializeGameObject(object data)
        {
            throw new NotImplementedException();
        }
    }

    public class ObjectPersister
    {
        public string SerializeObjectState(object comp)
        {
            throw new NotImplementedException();
        }

        public void DeserializeObjectState(object comp2, object data)
        {
            throw new NotImplementedException();
        }
    }
}