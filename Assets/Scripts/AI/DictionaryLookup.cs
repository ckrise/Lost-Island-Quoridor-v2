﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

static class DictionaryLookup
{
    //Dictionary 1: Wall Placed as a String : List of Pawn moves blocked by the wall.

    private static readonly Dictionary<string, List<Move>> PawnBlockLookup = new Dictionary<string, List<Move>>()
    {
        {"a1v", new List<Move>(new Move[] { new Move("a1", "b1"), new Move("a2", "b2") })},
        {"a2v", new List<Move>(new Move[] { new Move("a2", "b2"), new Move("a3", "b3") })},
        {"a3v", new List<Move>(new Move[] { new Move("a3", "b3"), new Move("a4", "b4") })},
        {"a4v", new List<Move>(new Move[] { new Move("a4", "b4"), new Move("a5", "b5") })},
        {"a5v", new List<Move>(new Move[] { new Move("a5", "b5"), new Move("a6", "b6") })},
        {"a6v", new List<Move>(new Move[] { new Move("a6", "b6"), new Move("a7", "b7") })},
        {"a7v", new List<Move>(new Move[] { new Move("a7", "b7"), new Move("a8", "b8") })},
        {"a8v", new List<Move>(new Move[] { new Move("a8", "b8"), new Move("a9", "b9") })},
        {"a1h", new List<Move>(new Move[] { new Move("a1", "a2"), new Move("b1", "b2") })},
        {"a2h", new List<Move>(new Move[] { new Move("a2", "a3"), new Move("b2", "b3") })},
        {"a3h", new List<Move>(new Move[] { new Move("a3", "a4"), new Move("b3", "b4") })},
        {"a4h", new List<Move>(new Move[] { new Move("a4", "a5"), new Move("b4", "b5") })},
        {"a5h", new List<Move>(new Move[] { new Move("a5", "a6"), new Move("b5", "b6") })},
        {"a6h", new List<Move>(new Move[] { new Move("a6", "a7"), new Move("b6", "b7") })},
        {"a7h", new List<Move>(new Move[] { new Move("a7", "a8"), new Move("b7", "b8") })},
        {"a8h", new List<Move>(new Move[] { new Move("a8", "a9"), new Move("b8", "b9") })},
        {"b1v", new List<Move>(new Move[] { new Move("b1", "c1"), new Move("b2", "c2") })},
        {"b2v", new List<Move>(new Move[] { new Move("b2", "c2"), new Move("b3", "c3") })},
        {"b3v", new List<Move>(new Move[] { new Move("b3", "c3"), new Move("b4", "c4") })},
        {"b4v", new List<Move>(new Move[] { new Move("b4", "c4"), new Move("b5", "c5") })},
        {"b5v", new List<Move>(new Move[] { new Move("b5", "c5"), new Move("b6", "c6") })},
        {"b6v", new List<Move>(new Move[] { new Move("b6", "c6"), new Move("b7", "c7") })},
        {"b7v", new List<Move>(new Move[] { new Move("b7", "c7"), new Move("b8", "c8") })},
        {"b8v", new List<Move>(new Move[] { new Move("b8", "c8"), new Move("b9", "c9") })},
        {"b1h", new List<Move>(new Move[] { new Move("b1", "b2"), new Move("c1", "c2") })},
        {"b2h", new List<Move>(new Move[] { new Move("b2", "b3"), new Move("c2", "c3") })},
        {"b3h", new List<Move>(new Move[] { new Move("b3", "b4"), new Move("c3", "c4") })},
        {"b4h", new List<Move>(new Move[] { new Move("b4", "b5"), new Move("c4", "c5") })},
        {"b5h", new List<Move>(new Move[] { new Move("b5", "b6"), new Move("c5", "c6") })},
        {"b6h", new List<Move>(new Move[] { new Move("b6", "b7"), new Move("c6", "c7") })},
        {"b7h", new List<Move>(new Move[] { new Move("b7", "b8"), new Move("c7", "c8") })},
        {"b8h", new List<Move>(new Move[] { new Move("b8", "b9"), new Move("c8", "c9") })},
        {"c1v", new List<Move>(new Move[] { new Move("c1", "d1"), new Move("c2", "d2") })},
        {"c2v", new List<Move>(new Move[] { new Move("c2", "d2"), new Move("c3", "d3") })},
        {"c3v", new List<Move>(new Move[] { new Move("c3", "d3"), new Move("c4", "d4") })},
        {"c4v", new List<Move>(new Move[] { new Move("c4", "d4"), new Move("c5", "d5") })},
        {"c5v", new List<Move>(new Move[] { new Move("c5", "d5"), new Move("c6", "d6") })},
        {"c6v", new List<Move>(new Move[] { new Move("c6", "d6"), new Move("c7", "d7") })},
        {"c7v", new List<Move>(new Move[] { new Move("c7", "d7"), new Move("c8", "d8") })},
        {"c8v", new List<Move>(new Move[] { new Move("c8", "d8"), new Move("c9", "d9") })},
        {"c1h", new List<Move>(new Move[] { new Move("c1", "c2"), new Move("d1", "d2") })},
        {"c2h", new List<Move>(new Move[] { new Move("c2", "c3"), new Move("d2", "d3") })},
        {"c3h", new List<Move>(new Move[] { new Move("c3", "c4"), new Move("d3", "d4") })},
        {"c4h", new List<Move>(new Move[] { new Move("c4", "c5"), new Move("d4", "d5") })},
        {"c5h", new List<Move>(new Move[] { new Move("c5", "c6"), new Move("d5", "d6") })},
        {"c6h", new List<Move>(new Move[] { new Move("c6", "c7"), new Move("d6", "d7") })},
        {"c7h", new List<Move>(new Move[] { new Move("c7", "c8"), new Move("d7", "d8") })},
        {"c8h", new List<Move>(new Move[] { new Move("c8", "c9"), new Move("d8", "d9") })},
        {"d1v", new List<Move>(new Move[] { new Move("d1", "e1"), new Move("d2", "e2") })},
        {"d2v", new List<Move>(new Move[] { new Move("d2", "e2"), new Move("d3", "e3") })},
        {"d3v", new List<Move>(new Move[] { new Move("d3", "e3"), new Move("d4", "e4") })},
        {"d4v", new List<Move>(new Move[] { new Move("d4", "e4"), new Move("d5", "e5") })},
        {"d5v", new List<Move>(new Move[] { new Move("d5", "e5"), new Move("d6", "e6") })},
        {"d6v", new List<Move>(new Move[] { new Move("d6", "e6"), new Move("d7", "e7") })},
        {"d7v", new List<Move>(new Move[] { new Move("d7", "e7"), new Move("d8", "e8") })},
        {"d8v", new List<Move>(new Move[] { new Move("d8", "e8"), new Move("d9", "e9") })},
        {"d1h", new List<Move>(new Move[] { new Move("d1", "d2"), new Move("e1", "e2") })},
        {"d2h", new List<Move>(new Move[] { new Move("d2", "d3"), new Move("e2", "e3") })},
        {"d3h", new List<Move>(new Move[] { new Move("d3", "d4"), new Move("e3", "e4") })},
        {"d4h", new List<Move>(new Move[] { new Move("d4", "d5"), new Move("e4", "e5") })},
        {"d5h", new List<Move>(new Move[] { new Move("d5", "d6"), new Move("e5", "e6") })},
        {"d6h", new List<Move>(new Move[] { new Move("d6", "d7"), new Move("e6", "e7") })},
        {"d7h", new List<Move>(new Move[] { new Move("d7", "d8"), new Move("e7", "e8") })},
        {"d8h", new List<Move>(new Move[] { new Move("d8", "d9"), new Move("e8", "e9") })},
        {"e1v", new List<Move>(new Move[] { new Move("e1", "f1"), new Move("e2", "f2") })},
        {"e2v", new List<Move>(new Move[] { new Move("e2", "f2"), new Move("e3", "f3") })},
        {"e3v", new List<Move>(new Move[] { new Move("e3", "f3"), new Move("e4", "f4") })},
        {"e4v", new List<Move>(new Move[] { new Move("e4", "f4"), new Move("e5", "f5") })},
        {"e5v", new List<Move>(new Move[] { new Move("e5", "f5"), new Move("e6", "f6") })},
        {"e6v", new List<Move>(new Move[] { new Move("e6", "f6"), new Move("e7", "f7") })},
        {"e7v", new List<Move>(new Move[] { new Move("e7", "f7"), new Move("e8", "f8") })},
        {"e8v", new List<Move>(new Move[] { new Move("e8", "f8"), new Move("e9", "f9") })},
        {"e1h", new List<Move>(new Move[] { new Move("e1", "e2"), new Move("f1", "f2") })},
        {"e2h", new List<Move>(new Move[] { new Move("e2", "e3"), new Move("f2", "f3") })},
        {"e3h", new List<Move>(new Move[] { new Move("e3", "e4"), new Move("f3", "f4") })},
        {"e4h", new List<Move>(new Move[] { new Move("e4", "e5"), new Move("f4", "f5") })},
        {"e5h", new List<Move>(new Move[] { new Move("e5", "e6"), new Move("f5", "f6") })},
        {"e6h", new List<Move>(new Move[] { new Move("e6", "e7"), new Move("f6", "f7") })},
        {"e7h", new List<Move>(new Move[] { new Move("e7", "e8"), new Move("f7", "f8") })},
        {"e8h", new List<Move>(new Move[] { new Move("e8", "e9"), new Move("f8", "f9") })},
        {"f1v", new List<Move>(new Move[] { new Move("f1", "g1"), new Move("f2", "g2") })},
        {"f2v", new List<Move>(new Move[] { new Move("f2", "g2"), new Move("f3", "g3") })},
        {"f3v", new List<Move>(new Move[] { new Move("f3", "g3"), new Move("f4", "g4") })},
        {"f4v", new List<Move>(new Move[] { new Move("f4", "g4"), new Move("f5", "g5") })},
        {"f5v", new List<Move>(new Move[] { new Move("f5", "g5"), new Move("f6", "g6") })},
        {"f6v", new List<Move>(new Move[] { new Move("f6", "g6"), new Move("f7", "g7") })},
        {"f7v", new List<Move>(new Move[] { new Move("f7", "g7"), new Move("f8", "g8") })},
        {"f8v", new List<Move>(new Move[] { new Move("f8", "g8"), new Move("f9", "g9") })},
        {"f1h", new List<Move>(new Move[] { new Move("f1", "f2"), new Move("g1", "g2") })},
        {"f2h", new List<Move>(new Move[] { new Move("f2", "f3"), new Move("g2", "g3") })},
        {"f3h", new List<Move>(new Move[] { new Move("f3", "f4"), new Move("g3", "g4") })},
        {"f4h", new List<Move>(new Move[] { new Move("f4", "f5"), new Move("g4", "g5") })},
        {"f5h", new List<Move>(new Move[] { new Move("f5", "f6"), new Move("g5", "g6") })},
        {"f6h", new List<Move>(new Move[] { new Move("f6", "f7"), new Move("g6", "g7") })},
        {"f7h", new List<Move>(new Move[] { new Move("f7", "f8"), new Move("g7", "g8") })},
        {"f8h", new List<Move>(new Move[] { new Move("f8", "f9"), new Move("g8", "g9") })},
        {"g1v", new List<Move>(new Move[] { new Move("g1", "h1"), new Move("g2", "h2") })},
        {"g2v", new List<Move>(new Move[] { new Move("g2", "h2"), new Move("g3", "h3") })},
        {"g3v", new List<Move>(new Move[] { new Move("g3", "h3"), new Move("g4", "h4") })},
        {"g4v", new List<Move>(new Move[] { new Move("g4", "h4"), new Move("g5", "h5") })},
        {"g5v", new List<Move>(new Move[] { new Move("g5", "h5"), new Move("g6", "h6") })},
        {"g6v", new List<Move>(new Move[] { new Move("g6", "h6"), new Move("g7", "h7") })},
        {"g7v", new List<Move>(new Move[] { new Move("g7", "h7"), new Move("g8", "h8") })},
        {"g8v", new List<Move>(new Move[] { new Move("g8", "h8"), new Move("g9", "h9") })},
        {"g1h", new List<Move>(new Move[] { new Move("g1", "g2"), new Move("h1", "h2") })},
        {"g2h", new List<Move>(new Move[] { new Move("g2", "g3"), new Move("h2", "h3") })},
        {"g3h", new List<Move>(new Move[] { new Move("g3", "g4"), new Move("h3", "h4") })},
        {"g4h", new List<Move>(new Move[] { new Move("g4", "g5"), new Move("h4", "h5") })},
        {"g5h", new List<Move>(new Move[] { new Move("g5", "g6"), new Move("h5", "h6") })},
        {"g6h", new List<Move>(new Move[] { new Move("g6", "g7"), new Move("h6", "h7") })},
        {"g7h", new List<Move>(new Move[] { new Move("g7", "g8"), new Move("h7", "h8") })},
        {"g8h", new List<Move>(new Move[] { new Move("g8", "g9"), new Move("h8", "h9") })},
        {"h1v", new List<Move>(new Move[] { new Move("h1", "i1"), new Move("h2", "i2") })},
        {"h2v", new List<Move>(new Move[] { new Move("h2", "i2"), new Move("h3", "i3") })},
        {"h3v", new List<Move>(new Move[] { new Move("h3", "i3"), new Move("h4", "i4") })},
        {"h4v", new List<Move>(new Move[] { new Move("h4", "i4"), new Move("h5", "i5") })},
        {"h5v", new List<Move>(new Move[] { new Move("h5", "i5"), new Move("h6", "i6") })},
        {"h6v", new List<Move>(new Move[] { new Move("h6", "i6"), new Move("h7", "i7") })},
        {"h7v", new List<Move>(new Move[] { new Move("h7", "i7"), new Move("h8", "i8") })},
        {"h8v", new List<Move>(new Move[] { new Move("h8", "i8"), new Move("h9", "i9") })},
        {"h1h", new List<Move>(new Move[] { new Move("h1", "h2"), new Move("i1", "i2") })},
        {"h2h", new List<Move>(new Move[] { new Move("h2", "h3"), new Move("i2", "i3") })},
        {"h3h", new List<Move>(new Move[] { new Move("h3", "h4"), new Move("i3", "i4") })},
        {"h4h", new List<Move>(new Move[] { new Move("h4", "h5"), new Move("i4", "i5") })},
        {"h5h", new List<Move>(new Move[] { new Move("h5", "h6"), new Move("i5", "i6") })},
        {"h6h", new List<Move>(new Move[] { new Move("h6", "h7"), new Move("i6", "i7") })},
        {"h7h", new List<Move>(new Move[] { new Move("h7", "h8"), new Move("i7", "i8") })},
        {"h8h", new List<Move>(new Move[] { new Move("h8", "h9"), new Move("i8", "i9") })}
    };
    
    //Function that performs the lookup on the wall data.
    public static List<Move> PerformPawnBlockLookup(string wallPlaced)
    {
        return PawnBlockLookup[wallPlaced];
    }


    //Dictionary 2: Wall as a string : List of Walls it prevents placement of
    //This is initialized with specific values after the first lookup..
    private static readonly Dictionary<string, List<string>> WallBlockLookup = new Dictionary<string, List<string>>()
    {
        {"a1v", new List<string>(new string[] { "a1v", "a1h", "a2v" })},
        {"a2v", new List<string>(new string[] { "a2v", "a2h", "a1v", "a3v" })},
        {"a3v", new List<string>(new string[] { "a3v", "a3h", "a2v", "a4v" })},
        {"a4v", new List<string>(new string[] { "a4v", "a4h", "a3v", "a5v" })},
        {"a5v", new List<string>(new string[] { "a5v", "a5h", "a4v", "a6v" })},
        {"a6v", new List<string>(new string[] { "a6v", "a6h", "a5v", "a7v" })},
        {"a7v", new List<string>(new string[] { "a7v", "a7h", "a6v", "a8v" })},
        {"a8v", new List<string>(new string[] { "a8v", "a8h", "a7v" })},
        {"a1h", new List<string>(new string[] { "a1h", "a1v", "b1h" })},
        {"a2h", new List<string>(new string[] { "a2h", "a2v", "b2h" })},
        {"a3h", new List<string>(new string[] { "a3h", "a3v", "b3h" })},
        {"a4h", new List<string>(new string[] { "a4h", "a4v", "b4h" })},
        {"a5h", new List<string>(new string[] { "a5h", "a5v", "b5h" })},
        {"a6h", new List<string>(new string[] { "a6h", "a6v", "b6h" })},
        {"a7h", new List<string>(new string[] { "a7h", "a7v", "b7h" })},
        {"a8h", new List<string>(new string[] { "a8h", "a8v", "b8h" })},
        {"b1v", new List<string>(new string[] { "b1v", "b1h", "b2v", })},
        {"b2v", new List<string>(new string[] { "b2v", "b2h", "b1v", "b3v" })},
        {"b3v", new List<string>(new string[] { "b3v", "b3h", "b2v", "b4v" })},
        {"b4v", new List<string>(new string[] { "b4v", "b4h", "b3v", "b5v" })},
        {"b5v", new List<string>(new string[] { "b5v", "b5h", "b4v", "b6v" })},
        {"b6v", new List<string>(new string[] { "b6v", "b6h", "b5v", "b7v" })},
        {"b7v", new List<string>(new string[] { "b7v", "b7h", "b6v", "b8v" })},
        {"b8v", new List<string>(new string[] { "b8v", "b8h", "b7v" })},
        {"b1h", new List<string>(new string[] { "b1h", "b1v", "a1h", "c1h" })},
        {"b2h", new List<string>(new string[] { "b2h", "b2v", "a2h", "c2h" })},
        {"b3h", new List<string>(new string[] { "b3h", "b3v", "a3h", "c3h" })},
        {"b4h", new List<string>(new string[] { "b4h", "b4v", "a4h", "c4h" })},
        {"b5h", new List<string>(new string[] { "b5h", "b5v", "a5h", "c5h" })},
        {"b6h", new List<string>(new string[] { "b6h", "b6v", "a6h", "c6h" })},
        {"b7h", new List<string>(new string[] { "b7h", "b7v", "a7h", "c7h" })},
        {"b8h", new List<string>(new string[] { "b8h", "b8v", "a8h", "c8h" })},
        {"c1v", new List<string>(new string[] { "c1v", "c1h", "c2v", })},
        {"c2v", new List<string>(new string[] { "c2v", "c2h", "c1v", "c3v" })},
        {"c3v", new List<string>(new string[] { "c3v", "c3h", "c2v", "c4v" })},
        {"c4v", new List<string>(new string[] { "c4v", "c4h", "c3v", "c5v" })},
        {"c5v", new List<string>(new string[] { "c5v", "c5h", "c4v", "c6v" })},
        {"c6v", new List<string>(new string[] { "c6v", "c6h", "c5v", "c7v" })},
        {"c7v", new List<string>(new string[] { "c7v", "c7h", "c6v", "c8v" })},
        {"c8v", new List<string>(new string[] { "c8v", "c8h", "c7v" })},
        {"c1h", new List<string>(new string[] { "c1h", "c1v", "b1h", "d1h" })},
        {"c2h", new List<string>(new string[] { "c2h", "c2v", "b2h", "d2h" })},
        {"c3h", new List<string>(new string[] { "c3h", "c3v", "b3h", "d3h" })},
        {"c4h", new List<string>(new string[] { "c4h", "c4v", "b4h", "d4h" })},
        {"c5h", new List<string>(new string[] { "c5h", "c5v", "b5h", "d5h" })},
        {"c6h", new List<string>(new string[] { "c6h", "c6v", "b6h", "d6h" })},
        {"c7h", new List<string>(new string[] { "c7h", "c7v", "b7h", "d7h" })},
        {"c8h", new List<string>(new string[] { "c8h", "c8v", "b8h", "d8h" })},
        {"d1v", new List<string>(new string[] { "d1v", "d1h", "d2v", })},
        {"d2v", new List<string>(new string[] { "d2v", "d2h", "d1v", "d3v" })},
        {"d3v", new List<string>(new string[] { "d3v", "d3h", "d2v", "d4v" })},
        {"d4v", new List<string>(new string[] { "d4v", "d4h", "d3v", "d5v" })},
        {"d5v", new List<string>(new string[] { "d5v", "d5h", "d4v", "d6v" })},
        {"d6v", new List<string>(new string[] { "d6v", "d6h", "d5v", "d7v" })},
        {"d7v", new List<string>(new string[] { "d7v", "d7h", "d6v", "d8v" })},
        {"d8v", new List<string>(new string[] { "d8v", "d8h", "d7v" })},
        {"d1h", new List<string>(new string[] { "d1h", "d1v", "c1h", "e1h" })},
        {"d2h", new List<string>(new string[] { "d2h", "d2v", "c2h", "e2h" })},
        {"d3h", new List<string>(new string[] { "d3h", "d3v", "c3h", "e3h" })},
        {"d4h", new List<string>(new string[] { "d4h", "d4v", "c4h", "e4h" })},
        {"d5h", new List<string>(new string[] { "d5h", "d5v", "c5h", "e5h" })},
        {"d6h", new List<string>(new string[] { "d6h", "d6v", "c6h", "e6h" })},
        {"d7h", new List<string>(new string[] { "d7h", "d7v", "c7h", "e7h" })},
        {"d8h", new List<string>(new string[] { "d8h", "d8v", "c8h", "e8h" })},
        {"e1v", new List<string>(new string[] { "e1v", "e1h", "e2v", })},
        {"e2v", new List<string>(new string[] { "e2v", "e2h", "e1v", "e3v" })},
        {"e3v", new List<string>(new string[] { "e3v", "e3h", "e2v", "e4v" })},
        {"e4v", new List<string>(new string[] { "e4v", "e4h", "e3v", "e5v" })},
        {"e5v", new List<string>(new string[] { "e5v", "e5h", "e4v", "e6v" })},
        {"e6v", new List<string>(new string[] { "e6v", "e6h", "e5v", "e7v" })},
        {"e7v", new List<string>(new string[] { "e7v", "e7h", "e6v", "e8v" })},
        {"e8v", new List<string>(new string[] { "e8v", "e8h", "e7v" })},
        {"e1h", new List<string>(new string[] { "e1h", "e1v", "d1h", "f1h" })},
        {"e2h", new List<string>(new string[] { "e2h", "e2v", "d2h", "f2h" })},
        {"e3h", new List<string>(new string[] { "e3h", "e3v", "d3h", "f3h" })},
        {"e4h", new List<string>(new string[] { "e4h", "e4v", "d4h", "f4h" })},
        {"e5h", new List<string>(new string[] { "e5h", "e5v", "d5h", "f5h" })},
        {"e6h", new List<string>(new string[] { "e6h", "e6v", "d6h", "f6h" })},
        {"e7h", new List<string>(new string[] { "e7h", "e7v", "d7h", "f7h" })},
        {"e8h", new List<string>(new string[] { "e8h", "e8v", "d8h", "f8h" })},
        {"f1v", new List<string>(new string[] { "f1v", "f1h", "f2v", })},
        {"f2v", new List<string>(new string[] { "f2v", "f2h", "f1v", "f3v" })},
        {"f3v", new List<string>(new string[] { "f3v", "f3h", "f2v", "f4v" })},
        {"f4v", new List<string>(new string[] { "f4v", "f4h", "f3v", "f5v" })},
        {"f5v", new List<string>(new string[] { "f5v", "f5h", "f4v", "f6v" })},
        {"f6v", new List<string>(new string[] { "f6v", "f6h", "f5v", "f7v" })},
        {"f7v", new List<string>(new string[] { "f7v", "f7h", "f6v", "f8v" })},
        {"f8v", new List<string>(new string[] { "f8v", "f8h", "f7v" })},
        {"f1h", new List<string>(new string[] { "f1h", "f1v", "e1h", "g1h" })},
        {"f2h", new List<string>(new string[] { "f2h", "f2v", "e2h", "g2h" })},
        {"f3h", new List<string>(new string[] { "f3h", "f3v", "e3h", "g3h" })},
        {"f4h", new List<string>(new string[] { "f4h", "f4v", "e4h", "g4h" })},
        {"f5h", new List<string>(new string[] { "f5h", "f5v", "e5h", "g5h" })},
        {"f6h", new List<string>(new string[] { "f6h", "f6v", "e6h", "g6h" })},
        {"f7h", new List<string>(new string[] { "f7h", "f7v", "e7h", "g7h" })},
        {"f8h", new List<string>(new string[] { "f8h", "f8v", "e8h", "g8h" })},
        {"g1v", new List<string>(new string[] { "g1v", "g1h", "g2v", })},
        {"g2v", new List<string>(new string[] { "g2v", "g2h", "g1v", "g3v" })},
        {"g3v", new List<string>(new string[] { "g3v", "g3h", "g2v", "g4v" })},
        {"g4v", new List<string>(new string[] { "g4v", "g4h", "g3v", "g5v" })},
        {"g5v", new List<string>(new string[] { "g5v", "g5h", "g4v", "g6v" })},
        {"g6v", new List<string>(new string[] { "g6v", "g6h", "g5v", "g7v" })},
        {"g7v", new List<string>(new string[] { "g7v", "g7h", "g6v", "g8v" })},
        {"g8v", new List<string>(new string[] { "g8v", "g8h", "g7v" })},
        {"g1h", new List<string>(new string[] { "g1h", "g1v", "f1h", "h1h" })},
        {"g2h", new List<string>(new string[] { "g2h", "g2v", "f2h", "h2h" })},
        {"g3h", new List<string>(new string[] { "g3h", "g3v", "f3h", "h3h" })},
        {"g4h", new List<string>(new string[] { "g4h", "g4v", "f4h", "h4h" })},
        {"g5h", new List<string>(new string[] { "g5h", "g5v", "f5h", "h5h" })},
        {"g6h", new List<string>(new string[] { "g6h", "g6v", "f6h", "h6h" })},
        {"g7h", new List<string>(new string[] { "g7h", "g7v", "f7h", "h7h" })},
        {"g8h", new List<string>(new string[] { "g8h", "g8v", "f8h", "h8h" })},
        {"h1v", new List<string>(new string[] { "h1v", "h1h", "h2v", })},
        {"h2v", new List<string>(new string[] { "h2v", "h2h", "h1v", "h3v" })},
        {"h3v", new List<string>(new string[] { "h3v", "h3h", "h2v", "h4v" })},
        {"h4v", new List<string>(new string[] { "h4v", "h4h", "h3v", "h5v" })},
        {"h5v", new List<string>(new string[] { "h5v", "h5h", "h4v", "h6v" })},
        {"h6v", new List<string>(new string[] { "h6v", "h6h", "h5v", "h7v" })},
        {"h7v", new List<string>(new string[] { "h7v", "h7h", "h6v", "h8v" })},
        {"h8v", new List<string>(new string[] { "h8v", "h8h", "h7v" })},
        {"h1h", new List<string>(new string[] { "h1h", "h1v", "g1h" })},
        {"h2h", new List<string>(new string[] { "h2h", "h2v", "g2h" })},
        {"h3h", new List<string>(new string[] { "h3h", "h3v", "g3h" })},
        {"h4h", new List<string>(new string[] { "h4h", "h4v", "g4h" })},
        {"h5h", new List<string>(new string[] { "h5h", "h5v", "g5h" })},
        {"h6h", new List<string>(new string[] { "h6h", "h6v", "g6h" })},
        {"h7h", new List<string>(new string[] { "h7h", "h7v", "g7h" })},
        {"h8h", new List<string>(new string[] { "h8h", "h8v", "g8h" })}
    };

//Function that performs the lookup on the wall data.
public static List<string> PerformWallBlockLookup(string wallPlaced)
{
    return WallBlockLookup[wallPlaced];
}

//Dictionary 3: Space as a string : List of spaces that are adjacent to the one given
//This is initialized with specific values after the first lookup.
private static readonly Dictionary<string, List<string>> AdjacentSpaceLookup = new Dictionary<string, List<string>>()
 {
        {"a1", new List<string> { "b1", "a2" }},
        {"a2", new List<string> { "b2", "a3", "a1" }},
        {"a3", new List<string> { "b3", "a4", "a2" }},
        {"a4", new List<string> { "b4", "a5", "a3" }},
        {"a5", new List<string> { "b5", "a6", "a4" }},
        {"a6", new List<string> { "b6", "a7", "a5" }},
        {"a7", new List<string> { "b7", "a8", "a6" }},
        {"a8", new List<string> { "b8", "a9", "a7" }},
        {"a9", new List<string> { "b9", "a8" }},
        {"b1", new List<string> { "a1", "c1", "b2" }},
        {"b2", new List<string> { "a2", "c2", "b3", "b1" }},
        {"b3", new List<string> { "a3", "c3", "b4", "b2" }},
        {"b4", new List<string> { "a4", "c4", "b5", "b3" }},
        {"b5", new List<string> { "a5", "c5", "b6", "b4" }},
        {"b6", new List<string> { "a6", "c6", "b7", "b5" }},
        {"b7", new List<string> { "a7", "c7", "b8", "b6" }},
        {"b8", new List<string> { "a8", "c8", "b9", "b7" }},
        {"b9", new List<string> { "a9", "c9", "b8" }},
        {"c1", new List<string> { "b1", "d1", "c2" }},
        {"c2", new List<string> { "b2", "d2", "c3", "c1" }},
        {"c3", new List<string> { "b3", "d3", "c4", "c2" }},
        {"c4", new List<string> { "b4", "d4", "c5", "c3" }},
        {"c5", new List<string> { "b5", "d5", "c6", "c4" }},
        {"c6", new List<string> { "b6", "d6", "c7", "c5" }},
        {"c7", new List<string> { "b7", "d7", "c8", "c6" }},
        {"c8", new List<string> { "b8", "d8", "c9", "c7" }},
        {"c9", new List<string> { "b9", "d9", "c8" }},
        {"d1", new List<string> { "c1", "e1", "d2" }},
        {"d2", new List<string> { "c2", "e2", "d3", "d1" }},
        {"d3", new List<string> { "c3", "e3", "d4", "d2" }},
        {"d4", new List<string> { "c4", "e4", "d5", "d3" }},
        {"d5", new List<string> { "c5", "e5", "d6", "d4" }},
        {"d6", new List<string> { "c6", "e6", "d7", "d5" }},
        {"d7", new List<string> { "c7", "e7", "d8", "d6" }},
        {"d8", new List<string> { "c8", "e8", "d9", "d7" }},
        {"d9", new List<string> { "c9", "e9", "d8" }},
        {"e1", new List<string> { "d1", "f1", "e2" }},
        {"e2", new List<string> { "d2", "f2", "e3", "e1" }},
        {"e3", new List<string> { "d3", "f3", "e4", "e2" }},
        {"e4", new List<string> { "d4", "f4", "e5", "e3" }},
        {"e5", new List<string> { "d5", "f5", "e6", "e4" }},
        {"e6", new List<string> { "d6", "f6", "e7", "e5" }},
        {"e7", new List<string> { "d7", "f7", "e8", "e6" }},
        {"e8", new List<string> { "d8", "f8", "e9", "e7" }},
        {"e9", new List<string> { "d9", "f9", "e8" }},
        {"f1", new List<string> { "e1", "g1", "f2" }},
        {"f2", new List<string> { "e2", "g2", "f3", "f1" }},
        {"f3", new List<string> { "e3", "g3", "f4", "f2" }},
        {"f4", new List<string> { "e4", "g4", "f5", "f3" }},
        {"f5", new List<string> { "e5", "g5", "f6", "f4" }},
        {"f6", new List<string> { "e6", "g6", "f7", "f5" }},
        {"f7", new List<string> { "e7", "g7", "f8", "f6" }},
        {"f8", new List<string> { "e8", "g8", "f9", "f7" }},
        {"f9", new List<string> { "e9", "g9", "f8" }},
        {"g1", new List<string> { "f1", "h1", "g2" }},
        {"g2", new List<string> { "f2", "h2", "g3", "g1" }},
        {"g3", new List<string> { "f3", "h3", "g4", "g2" }},
        {"g4", new List<string> { "f4", "h4", "g5", "g3" }},
        {"g5", new List<string> { "f5", "h5", "g6", "g4" }},
        {"g6", new List<string> { "f6", "h6", "g7", "g5" }},
        {"g7", new List<string> { "f7", "h7", "g8", "g6" }},
        {"g8", new List<string> { "f8", "h8", "g9", "g7" }},
        {"g9", new List<string> { "f9", "h9", "g8" }},
        {"h1", new List<string> { "g1", "i1", "h2" }},
        {"h2", new List<string> { "g2", "i2", "h3", "h1" }},
        {"h3", new List<string> { "g3", "i3", "h4", "h2" }},
        {"h4", new List<string> { "g4", "i4", "h5", "h3" }},
        {"h5", new List<string> { "g5", "i5", "h6", "h4" }},
        {"h6", new List<string> { "g6", "i6", "h7", "h5" }},
        {"h7", new List<string> { "g7", "i7", "h8", "h6" }},
        {"h8", new List<string> { "g8", "i8", "h9", "h7" }},
        {"h9", new List<string> { "g9", "i9", "h8" }},
        {"i1", new List<string> { "h1", "i2" }},
        {"i2", new List<string> { "h2", "i3", "i1" }},
        {"i3", new List<string> { "h3", "i4", "i2" }},
        {"i4", new List<string> { "h4", "i5", "i3" }},
        {"i5", new List<string> { "h5", "i6", "i4" }},
        {"i6", new List<string> { "h6", "i7", "i5" }},
        {"i7", new List<string> { "h7", "i8", "i6" }},
        {"i8", new List<string> { "h8", "i9", "i7" }},
        {"i9", new List<string> { "h9", "i8" }},
    };


    //Function that performs the lookup and gives a list of adjacent spaces.
    public static List<string> PerformAdjacentSpaceLookup(string space)
    {
        return AdjacentSpaceLookup[space];
    }
}
