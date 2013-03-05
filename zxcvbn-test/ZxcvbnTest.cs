﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace zxcvbn_test
{
    [TestClass]
    public class ZxcvbnTest
    {
        private static string[] testPasswords = new string[] {
			"zxcvbn",
			"qwER43@!",
			"Tr0ub4dour&3",
			"correcthorsebatterystaple",
			"coRrecth0rseba++ery9.23.2007staple$",
			"D0g..................",
			"abcdefghijk987654321",
			"neverforget13/3/1997",
			"1qaz2wsx3edc",
			"temppass22",
			"briansmith",
			"briansmith4mayor",
			"password1",
			"viking",
			"thx1138",
			"ScoRpi0ns",
			"do you know",
			"ryanhunter2000",
			"rianhunter2000",
			"asdfghju7654rewq",
			"AOEUIDHG&*()LS_",
			"12345678",
			"defghi6789",
			"rosebud",
			"Rosebud",
			"ROSEBUD",
			"rosebuD",
			"ros3bud99",
			"r0s3bud99",
			"R0$38uD99",
			"verlineVANDERMARK",
			"eheuczkqyq",
			"rWibMFACxAUGZmxhVncy",
			"Ba9ZyWABu99[BK#6MBgbH88Tofv)vs$w"
        };

        [TestMethod]
        public void RunAllTestPasswords()
        {
            foreach (var password in testPasswords)
            {
                var result = Zxcvbn.Zxcvbn.MatchPassword(password);
            }
        }
    }
}
