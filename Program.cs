﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinaryTree
{
    internal class Node
    {
        public Node Right { get; } // branche de droite
        public Node Left { get; } // branche de gauche

        public char Letter { get; } // Si feuille

        public string Signature { get; set; } // Code binaire selon Huffman
        public int Weight { get; } // poids du noeud

        // Constructeur noeud
        public Node(Node right, Node left, int weight)
        {
            Right = right;
            Left = left;
            Weight = weight;
        }

        // Constructeur feuille
        public Node(char letter, int weight)
        {
            Letter = letter;
            Weight = weight;
        }

        // Fonction de lecture de l'arbre de manière récursive
        // et inscription dans le tableau de correspondance
        public void RecursiveReading()
        {
            if (Letter == '\0')
            {
                // Parcourt les branches de droite
                Left.Signature = Signature + "0"; // La signature du noeud courant avec un 0 en plus
                Left.RecursiveReading();
                
                // Parcourt les branches de gauche
                Right.Signature = Signature + "1"; // La signature du noeud courant avec un 1 en plus
                Right.RecursiveReading();
            }
            else
            {
                // Écrit la correspondance dans la table
                BinaryTree.AddressMappingTable[Signature] = Letter;
            }
        }
    }

    internal class BinaryTree
    {
        public static Dictionary<string, char> AddressMappingTable = new Dictionary<string, char>(); // Tableau de correspondance

        public List<Node> Nodes; // Liste de noeuds

        // Trie les noeuds par poids
        public void SortNodesByWeight()
        {
            Nodes.Sort(
                (n1, n2) => n1.Weight.CompareTo(n2.Weight)
            );
        }

        // Parcourt l'arbre de manière itérative
        public void IterativeReading()
        {
            var nodeList = new List<Node> {Nodes[0]}; // Faire une liste de noeuds à traiter

            while (nodeList.Count > 0)
            {
                if (nodeList[0].Letter != '\0') // Tester si c'est une feuille
                {
                    AddressMappingTable[nodeList[0].Signature] = nodeList[0].Letter;
                }
                else
                {
                    nodeList[0].Right.Signature = nodeList[0].Signature + "1";
                    nodeList.Add(nodeList[0].Right);

                    nodeList[0].Left.Signature = nodeList[0].Signature + "0";
                    nodeList.Add(nodeList[0].Left);
                }
                nodeList.RemoveAt(0); // Supprimer le noeud traité
            }
        }

        // Affiche les poids dans la console
        public void PrintWeights()
        {
            Nodes.ForEach(delegate(Node node)
            {
                Console.Write(node.Letter);
                Console.Write("    ");
                Console.WriteLine(node.Weight);
            });
            Console.WriteLine(Nodes.Count);
            Console.WriteLine("-----------------");
        }

        // Affiche la table de correpondance dans la console
        public void PrintMappingTable()
        {
            foreach (var letter in AddressMappingTable)
            {
                Console.WriteLine("Human = {0}    Huffman = {1}", letter.Value, letter.Key);
            }
        }
    }

    class Utils
    {
        // Fonction de décodage du code binaire
        public static string HuffmanDecode(string encodedMessage, Dictionary<string, char> mappingTable)
        {
            var decodedMessage = "";
            var length = 1; // nombre de bits évalués
            while (encodedMessage.Length > 0)
            {
                var evaluatedCode = encodedMessage.Substring(0, length); // code évalué

                // Vérifier si une lettre correspond
                foreach (var letter in mappingTable)
                {
                    if (letter.Key == evaluatedCode)
                    {
                        decodedMessage += letter.Value;
                        encodedMessage = encodedMessage.Substring(length);
                        length = 1;
                    }
                }
                length++; // Rajouter un bit à la taille
            }
            return decodedMessage;
        }
        
        // Encoder et afficher le message encodé
        public static string EncodeAndPrintEncodedMessage(char[] inputArray)
        {
            var encodedMessage = "";
            
            foreach (var letter in inputArray)
            {
                encodedMessage += BinaryTree.AddressMappingTable.FirstOrDefault(x => x.Value == letter).Key;
            }
            
            Console.WriteLine("Encoded message:");
            Console.WriteLine(encodedMessage);
            Console.WriteLine("Number of bits: {0}", encodedMessage.Length);

            Console.WriteLine("---------------------------------------------------");

            return encodedMessage;
        }
        
        // Décoder et afficher le message décodé
        public static string DecodeAndPrintDecodedMessage(string encodedMessage, Dictionary<string, char> mappingTable)
        {
            Console.WriteLine("Decoded message:");
            
            string decodedMessage = HuffmanDecode(encodedMessage, BinaryTree.AddressMappingTable);
            Console.WriteLine(decodedMessage);
            Console.WriteLine("Number of bits: {0}", Encoding.UTF8.GetByteCount(decodedMessage));
            
            Console.WriteLine("---------------------------------------------------");

            return decodedMessage;
        }
        
        // Calculer et afficher la taille compressée en % de la taille d'origine
        public static double CalculateAndPrintCompressionRatio(string encodedMessage, string decodedMessage)
        {
            double encodedMessLength = encodedMessage.Length;
            double decodedMessLength = Encoding.UTF8.GetByteCount(decodedMessage) * 8;

            double ratio = encodedMessLength / decodedMessLength * 100;
            
            Console.WriteLine("Original length in bits: {0}, After encoding: {1}", decodedMessLength, encodedMessLength);
            Console.WriteLine("Ratio: {0}%", Math.Round(ratio, 2));

            return ratio;
        }
    }

    internal class Program
    {
        public static void Main(string[] args)
        {
            // Message d'origine
            var input = "The quick brown fox jumps over the lazy dog !!!";
            var inputArray = input.ToCharArray();

            var occurenceCount = new Dictionary<char, int>();


            // Crée le tableau des occurences
            foreach (var letter in inputArray)
            {
                if (occurenceCount.ContainsKey(letter))
                {
                    occurenceCount[letter] += 1;
                }
                else
                {
                    occurenceCount[letter] = 1;
                }
            }

            // Initialiser l'arbre
            var binaryTree = new BinaryTree
            {
                Nodes = occurenceCount.Select(letter => new Node(letter.Key, letter.Value)).ToList()
            };
            binaryTree.SortNodesByWeight();
            binaryTree.PrintWeights();

            // Créer les nouveaux noeuds
            while (binaryTree.Nodes.Count > 1)
            {
                var weight = binaryTree.Nodes[0].Weight + binaryTree.Nodes[1].Weight;
                binaryTree.Nodes.Add(new Node(binaryTree.Nodes[0], binaryTree.Nodes[1], weight));

                // Debug
                //Console.WriteLine("min1:" + binaryTree.Nodes[0].letter);
                //Console.WriteLine("min2:" + binaryTree.Nodes[1].letter);

                // Supprimer les noeuds assemblés
                binaryTree.Nodes.Remove(binaryTree.Nodes[1]);
                binaryTree.Nodes.Remove(binaryTree.Nodes[0]);

                // Trier et afficher
                binaryTree.SortNodesByWeight();
                binaryTree.PrintWeights();
            }

            // Afficher poids total
            Console.WriteLine(binaryTree.Nodes[0].Weight);

            // Créer et afficher le tableau de correspondance
            //binaryTree.Nodes[0].RecursiveReading();
            binaryTree.IterativeReading();

            if (BinaryTree.AddressMappingTable.Count == 0)
            {
                Console.WriteLine("Nothing has been written in mapping table");
            }
            else
            {
                binaryTree.PrintMappingTable();
            }

            Console.WriteLine("---------------------------------------------------");

            // Encoder et afficher le message d'origine encodé
            var encodedMessage = Utils.EncodeAndPrintEncodedMessage(inputArray);

            // Décoder et afficher le message d'origine
            var decodedMessage = Utils.DecodeAndPrintDecodedMessage(encodedMessage, BinaryTree.AddressMappingTable);
            
            // Calculer et afficher le taux de compression
            Utils.CalculateAndPrintCompressionRatio(encodedMessage, decodedMessage);
        }
    }
}