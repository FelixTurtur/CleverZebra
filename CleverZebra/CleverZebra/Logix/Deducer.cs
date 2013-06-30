﻿using CleverZebra.Representation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleverZebra.Logix
{
    class Deducer
    {
        private RelationFactory relationBuilder;
        private List<Line> lines;
        private int puzzleSize;

        public Deducer(int size) {
            this.puzzleSize = size;
            this.relationBuilder = RelationFactory.getInstance();
            char c = 'A';
            for (int i = 0; i < puzzleSize; i++, c++) {
                this.lines.Add(new Line(c, puzzleSize));
            }
        }

        private Line getLineFromIdentifier(char ident) {
            foreach (Line l in this.lines) {
                if (l.identifier == ident) { return l; }
            }
            throw new ArgumentException("No Line found for identifier: " + ident);
        }

        public Relation[] considerRelationToLine(Relation r, Line l) {
            if (r.getBaseItem(l.identifier) == null) {
                //cannot use this relation
                return new Relation[] {r};
            }
            if (!r.isRelative()) {
                //direct relation
                //if either side is this line's category, add.
                Line.Rows row = r.isPositive() ? Line.Rows.Positives : Line.Rows.Negatives;
                l.addRelation(r.getBaseItem(l.identifier), r.getRelatedItem(l.identifier), row);
                addInverse(r.getRelatedItem(l.identifier), r.getBaseItem(l.identifier), row);
                return null;
            }
            //relative relation
            //if either of the two items has a value, then something can be learnt for the other, if not a complete match
            string[] items = { r.getBaseItem(l.identifier, Relations.Sides.Left), r.getBaseItem(l.identifier, Relations.Sides.Right) };
            string leftMatch = l.checkForMatch(items[0]);
            string rightMatch = l.checkForMatch(items[1]);
            if (!string.IsNullOrEmpty(leftMatch) && !string.IsNullOrEmpty(rightMatch)) {
                //both sides already matched 
                return null;
            } 
            else if (!string.IsNullOrEmpty(leftMatch) || !string.IsNullOrEmpty(rightMatch)) {
                if (Representation.Relations.isQuantified(r.getRule())) {
                    string unknownItem = leftMatch == null ? items[0] : items[1];
                    object knownValue = l.retrieveValue(leftMatch==null ? rightMatch : leftMatch);
                    object targetValue = calculateRelatedValue(knownValue, r.getRule());
                    string targetItem = l.findValue(targetValue);
                    l.addRelation(targetItem, unknownItem , r.isPositive() ? Line.Rows.Positives : Line.Rows.Negatives);
                }
            } 
            else if (eitherSideMatches(r, l, Line.Rows.Negatives)) {
            }
            return null;
        }

        private object calculateRelatedValue(object knownValue, string p) {
            throw new NotImplementedException();
        }

        private void addInverse(string p1, string p2, Line.Rows row) {
            getLineFromIdentifier(p1[0]).addRelation(p1, p2, row);
        }

        private static bool eitherSideMatches(Relation r, Line l, Line.Rows row) {
            string leftMatch = l.checkForMatch(r.getBaseItem(l.identifier, Relations.Sides.Left), row);
            string rightMatch = l.checkForMatch(r.getBaseItem(l.identifier, Relations.Sides.Right), row);
            return !string.IsNullOrEmpty(leftMatch) || !string.IsNullOrEmpty(rightMatch);
        }


    }
}
