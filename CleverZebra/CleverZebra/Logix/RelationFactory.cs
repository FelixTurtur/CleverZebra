﻿using CleverZebra.Representation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleverZebra.Logix
{
    public class RelationFactory
    {
        static RelationFactory instance = null;

        public RelationFactory() { }

        public static RelationFactory getInstance() {
            if (instance == null) {
                instance = new RelationFactory();
            }
            return instance;
        }

        public Relation createRelation(string input) {
            if (Relations.isRelative(input)) {
                return new RelativeRelation(input); 
            }
            return new DirectRelation(input);
        }

    }
}