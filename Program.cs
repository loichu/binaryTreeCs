using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.Linq;

namespace BinaryTree
{
    class Node
    {
        public Node right { get; } // branche de droite
        public Node left { get; } // branche de gauche

        public char letter { get; } // Si feuille

        public string signature { get; set; } // Code binaire selon Huffman
        public int weight { get; } // poids du noeud
        public int depth { get; set; } // profondeur dans l'arbre

        // Constructeur noeud
        public Node(Node right, Node left,  int weight)
        {
            this.right = right;
            this.left = left;
            this.weight = weight;
        }

        // Constructeur feuille
        public Node(char letter, int weight)
        {
            this.letter = letter;
            this.weight = weight;
        }

        // Fonction de lecture de l'arbre de manière récursive
        // et inscription dans le tableau de correspondance
        public void RecursiveReading()
        {
            if (this.letter == '\0')
            {
                this.left.signature = this.signature;
                this.left.signature += 0; // La signature du noeud courant avec un 0 en plus
                this.left.depth = this.depth + 1; // Ajouter la profondeur
                this.right.signature = this.signature;
                this.right.signature += 1; // La signature du noeud courant avec un 1 en plus
                this.right.depth = this.depth + 1; // Ajouter la profondeur
                this.left.RecursiveReading();
                this.right.RecursiveReading();
            }
            else
            {
                // Écrit la correspondance dans la table
                BinaryTree.addressMappingTable[signature] = letter;
            }
        }
    }

    class BinaryTree
    {
        public static Dictionary<string, char> addressMappingTable = new Dictionary<string, char>(); // Tableau de correspondance

        public List<Node> Nodes; // Liste de noeuds

        // Trie les noeuds par poids
        public void SortNodesByWeight()
        {
            Nodes.Sort(
                (n1, n2) => n1.weight.CompareTo(n2.weight)
            );
        }

        // Parcourt l'arbre de manière ittérative
        public void ItterativeReading()
        {
            List<Node> nodeList = new List<Node>(); // Faire une liste de noeuds à traiter

            nodeList.Add(Nodes[0]);

            while (nodeList.Count > 0)
            {
                if (nodeList[0].letter != '\0') // Tester si c'est une feuille
                {
                    addressMappingTable[nodeList[0].signature] = nodeList[0].letter;
                }
                else
                {
                    nodeList[0].right.signature = nodeList[0].signature + "1";
                    nodeList.Add(nodeList[0].right);

                    nodeList[0].left.signature = nodeList[0].signature + "0";
                    nodeList.Add(nodeList[0].left);
                }
                nodeList.RemoveAt(0); // Supprimer le noeud traité
            }
        }

        // Affiche les poids dans la console
        public void PrintWeights()
        {
            Nodes.ForEach(delegate(Node node)
            {
                Console.Write(node.letter);
                Console.Write("    ");
                Console.WriteLine(node.weight);
            });
            Console.WriteLine(Nodes.Count);
            Console.WriteLine("-----------------");
        }

        // Affiche la table de correpondance dans la console
        public void PrintMappingTable()
        {
            foreach (var letter in addressMappingTable)
            {
                Console.WriteLine("Human = {0}    Huffman = {1}", letter.Value, letter.Key);
            }
        }
    }

    internal class Program
    {

        // Fonction de décodage du code binaire
        private static string HuffmanDecode(string encodedMessage)
        {
            var decodedMessage = "";
            var length = 1; // nombre de bits évalués
            while(encodedMessage.Length > 0)
            {
                var evaluatedCode = encodedMessage.Substring(0, length); // code évalué

                // Vérifier si une lettre correspond
                foreach (var letter in BinaryTree.addressMappingTable)
                {
                    if (letter.Key == evaluatedCode)
                    {
                        decodedMessage += letter.Value;
                        encodedMessage = encodedMessage.Substring(length);
                        length = 1;
                    }
                }
                length++; // Rajouter un bit
            }
            return decodedMessage;
        }

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
                var weight = binaryTree.Nodes[0].weight + binaryTree.Nodes[1].weight;
                binaryTree.Nodes.Add(new Node(binaryTree.Nodes[0], binaryTree.Nodes[1], weight));

                // Debug
                Console.WriteLine("min1:" + binaryTree.Nodes[0].letter);
                Console.WriteLine("min2:" + binaryTree.Nodes[1].letter);

                // Supprimer les noeuds assemblés
                binaryTree.Nodes.Remove(binaryTree.Nodes[1]);
                binaryTree.Nodes.Remove(binaryTree.Nodes[0]);

                // Trier et afficher
                binaryTree.SortNodesByWeight();
                binaryTree.PrintWeights();
            }

            // Afficher poids total
            Console.WriteLine(binaryTree.Nodes[0].weight);

            // Créer et afficher le tableau de correspondance
            //binaryTree.Nodes[0].RecursiveReading();
            binaryTree.ItterativeReading();

            if (BinaryTree.addressMappingTable.Count == 0)
            {
                Console.WriteLine("ALERT");
            }
            else
            {
                binaryTree.PrintMappingTable();
            }

            // Encoder et afficher le message d'origine encodé
            var compressedMessage = "";
            foreach (var letter in inputArray)
            {
                compressedMessage += BinaryTree.addressMappingTable.FirstOrDefault(x => x.Value == letter).Key;
            }
            Console.WriteLine(compressedMessage);

            // Décoder et afficher le message d'origine
            Console.WriteLine(HuffmanDecode(compressedMessage));
        }
    }
}