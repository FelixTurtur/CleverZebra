﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CZParser;
using Logix;
using Representation;

namespace CleverZebra {
    internal delegate void SolverBoxUpdateHandler(Controller c, SolutionBoxEventArgs e);

    internal class Controller {

        #region Singletonia
        private static Controller instance = null;
        private Controller() {}
        public static Controller getInstance() {
            if (instance == null) {
                instance = new Controller();
            }
            return instance;
        }
        #endregion

        private List<Puzzle> puzzles;
        private Puzzle activePuzzle;
        internal SolverBoxUpdateHandler Updater;

        public void loadPuzzles(XmlDocument sourceDoc = null) {
            if (sourceDoc == null) {
                sourceDoc = new XmlDocument();
                sourceDoc.LoadXml(CleverZebra.Properties.Resources.puzzles);
            }
            else checkValidFile(sourceDoc);
            sourceDoc.Normalize();
            puzzles = new List<Puzzle>();
            XmlNodeList xmlPuzzles = sourceDoc.GetElementsByTagName("puzzle");
            foreach (XmlNode n in xmlPuzzles) {
                if (string.IsNullOrEmpty(n["title"].InnerText)) {
                    continue;
                }
                puzzles.Add(new Puzzle(n));
            }
        }

        private void checkValidFile(XmlDocument sourceDoc) {
            throw new NotImplementedException();
        }

        public void Solve(Puzzle puzzle = null) {
            if (puzzle != null) {
                activePuzzle = puzzle;
            }
            try {
                Parser parser = new Parser(activePuzzle);
                List<string> rules = parser.Read();
                activePuzzle.setRules(rules);
            }
            catch (Exception e) {
                throw new CZParser.ParserException("Unable to parse clues for puzzle id: " + activePuzzle.getId(), e);
            }
            List<List<string>> solution = null;
            Representation.Results stats = null;
            try {
                Logix.Logix logix = new Logix.Logix();
                logix.updater += logix_update;
                solution = logix.Solve(activePuzzle);
                stats = logix.getLastResults();
            }
            catch (Exception e) {
                throw new Logix.LogicException("Unable to solve puzzle id: " + activePuzzle.getId(), e);
            }
            reportSuccess(solution, stats);
        }

        public void logix_update(Logix.Logix sender, SolutionBoxEventArgs e) {
            if (Updater != null) {
                Updater(this, e);
            }
        }

        private void reportSuccess(List<List<string>> solution, Results stats) {
            List<List<string>> originalSolution = activePuzzle.ProvidedSolution;
            if (solution == originalSolution) {
                Console.WriteLine("We have achieved a great victory.");
            }
            Console.WriteLine("Well, that's not *quite* what I was going for.");
        }

        internal void setActivePuzzle(int p) {
            this.activePuzzle = puzzles[p];
        }

        internal string getPreamble() {
            return this.activePuzzle.getPreamble();
        }

        internal List<string> getPuzzleTitles() {
            List<string> titles = new List<string>();
            foreach (Puzzle p in puzzles) {
                titles.Add(p.name);
            }
            return titles;
        }

        internal string getActiveTitle() {
            return this.activePuzzle.name;
        }

        internal string getActivePreamble() {
            return this.activePuzzle.getPreamble();
        }

        internal List<string> getActiveClues() {
            return this.activePuzzle.getClues();
        }

        internal int getActiveWidth() {
            return this.activePuzzle.width;
        }

        internal int getActiveHeight() {
            return this.activePuzzle.height;
        }

        internal List<string> getCategoryTitles() {
            return this.activePuzzle.getCategories();
        }
    }
}
