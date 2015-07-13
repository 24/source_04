using System;

namespace Print
{
    [Flags]
    public enum LeMondeType
    {
        // ARG : argent
        // ARH : culture
        // DOS : dossier
        // MDE : économie, éco
        // EDU : éducation, édu
        // AUT : festival
        // PEH : géo-politique, géo
        // LIV : livres
        // MAG : magazine, mag
        // MDV : mode
        // SCH : science
        // SCQ : éco & entreprise, éco
        // SPH : sport
        // STY : style
        // NYT : the newyork times, nyt
        // TEL : tv
        // document (Le monde - 2012-10-24 - no 21076 -document.pdf)
        // élection (Le monde - 2012-05-08 - no 20931 -élection.pdf)
        // hebdo (Le monde - 2012-02-25 - no 20870 -hebdo.pdf)
        // sup (Le monde - 2008-11-06 - no 19838 -sup.pdf)
        // |document|élection|hebdo|sup
        Unknow = 0,
        Quotidien             = 0x0000001,
        Argent                = 0x0000002,
        Culture               = 0x0000004,
        Dossier               = 0x0000008,
        Economie              = 0x0000010,
        Education             = 0x0000020,
        Festival              = 0x0000040,
        GeoPolitique          = 0x0000080,
        Livres                = 0x0000100,
        Magazine              = 0x0000200,
        Mode                  = 0x0000400,
        Science               = 0x0000800,
        Sport                 = 0x0001000,
        Style                 = 0x0002000,
        TheNewyorkTimes       = 0x0004000,
        TV                    = 0x0008000,
        Document              = 0x0010000,
        Election              = 0x0020000,
        SelectionHebdomadaire = 0x0040000,
        Supplement            = 0x0080000
    }
}
