ListToPol[l_]:= FromDigits[Reverse[l],x];

ConvertToPols[l_]:=
(
    pols = {};

    For[i = 1, i <= Length[l], i++,
        tmp = l[[i]];
        pols = Append[pols, ListToPol[tmp]];
    ];

    pols
);

FactorPols[pl_, p_]:=
(
    factrs = {};

    For[i = 1, i <= Length[pl], i++,
        pol = pl[[i]];
        factrs = Append[factrs, FactorList[pol, Modulus->p]];
    ];

    factrs
);

ConvertFactorsToOutput[l_]:=
(
    factrsStr = {};

    For[i = 1, i <= Length[l], i++,
        tmpfl = l[[i]];

        tmpfactrsStr = {};

        For[j = 1, j <= Length[tmpfl], j++,
            t = tmpfl[[j]];

            Print[t];
            
            prt =
            ToString[
                InputForm[t[[1]]]
            ]
            <> FromCharacterCode[59] <>
            ToString[
                InputForm[t[[2]]]
            ];

            tmpfactrsStr = Append[tmpfactrsStr, prt];
        ];

        factrsStr = Append[factrsStr, tmpfactrsStr];
    ];

    factrsStr
);

ConvertFactorsToJson[l_]:=
(
    factrsJson = {};

    For[i = 1, i <= Length[l], i++,
        tmpfl = l[[i]];

        tmpfactrsJson = {};

        For[j = 1, j <= Length[tmpfl], j++,
            t = tmpfl[[j]];

            pol = t[[1]];
            exp = t[[2]];

            tmpList =
            {
                "pol"->ToString[InputForm[pol]],
                "exp"->ToString[InputForm[exp]]
            };

            tmpfactrsJson = Append[tmpfactrsJson, tmpList];
        ];

        factrsJson = Append[factrsJson, tmpfactrsJson];
    ];

    factrsJson
);

(* Main function starts here *)

(* field order *)
p = ToExpression[Import["fieldOrder_input.txt"]];

polsList = ToExpression[StringSplit[Import["pols_input.txt"], "\n"]];
pols = ConvertToPols[polsList];
factorPols = FactorPols[pols, p];
outputRes = ConvertFactorsToJson[factorPols];

Export["pols_output.json", outputRes];

Print[True];