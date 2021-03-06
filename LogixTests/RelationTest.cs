﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Logix;

namespace LogixTests
{
    [TestClass]
    public class RelationTest
    {
        private RelationFactory relationBuilder;

        [TestInitialize]
        public void Initialize() {
            this.relationBuilder = RelationFactory.getInstance();
        }
        
        [TestMethod]
        public void Create_Correct_Relation_Type() {
            Relation r = relationBuilder.createRelation("A1(B)-A3(B)=5");
            Assert.AreEqual(r.GetType().Name, "RelativeRelation");
            Assert.IsTrue(r.isRelative());
            r = relationBuilder.createRelation("A1=B3");
            Assert.AreEqual(r.GetType().Name, "DirectRelation");
            Assert.IsTrue(r.isDirect());
            r = relationBuilder.createRelation("?A1=B2?A1(C)>B1(C):A1(C)>B2(C)");
            Assert.AreEqual(r.GetType().Name, "ConditionalRelation");
            Assert.IsTrue(r.isConditional());
        }

        [TestMethod]
        public void Detect_Positive_Or_Negative() {
            Relation r = relationBuilder.createRelation("A1=B3");
            Assert.IsTrue(r.isPositive());
            r = relationBuilder.createRelation("A1!=B3");
            Assert.IsFalse(r.isPositive());
        }

        [TestMethod]
        public void Item_Retrieval() {
            Relation r = relationBuilder.createRelation("A1=B3");
            string bItem = r.getBaseItem('B');
            Assert.AreEqual("B3", bItem);
            string aItem = r.getRelatedItem('B');
            Assert.AreEqual("A1", aItem);
        }

        [TestMethod]
        public void Check_Representation() {
            Relation r = relationBuilder.createRelation("A1(C)-B1(C)=5");
            Assert.IsTrue(Representation.Relations.isQuantified(r.getRule()));
            r = relationBuilder.createRelation("A1(C)>B1(C)");
            Assert.IsFalse(Representation.Relations.isQuantified(r.getRule()));
        }
    }
}
