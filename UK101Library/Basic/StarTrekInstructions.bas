10 FORI=1TO16:PRINT:NEXT
20 PRINT"      INSTRUCTIONS FOR STARTREK"
30 PRINT"      -------------------------":PRINT
40 PRINT"IN STARTREK YOU ARE CAPTAIN OF THE STAR SHIP"
50 PRINT"ENTERPRISE TRAVELLING THROUGH THE GALAXY"
60 PRINT"HUNTING DOWN AND DESTROYING THE EVIL KLINGONS."
70 PRINT:PRINT"YOU HAVE 120 'STARDATES' TO FIND AND DESTROY"
80 PRINT"ALL THE KLINGONS, BEFORE THEY DEVELOP THEIR"
90 PRINT"SECRET WEAPON AND TAKE OVER THE GALAXY,"
100 PRINT"ENSLAVE HUMANITY ETC. ETC."
110 PRINT:PRINT:GOSUB9000
120 PRINT:PRINT:PRINT"YOUR SHIP IS ARMED WITH PHOTON TORPEDOES (8)"
130 PRINT"AND PHASARS, YOU ALSO HAVE VARIOUS SCANNERS"
140 PRINT"WITH WHICH TO LOCATE THE KLINGONS."
150 PRINT:PRINT"YOU DRIVE YOUR SHIP BY REQUESTING"
160 PRINT"VARIOUS 'FUNCTIONS' WHEN YOU SEE 'FUNCTION?'"
190 PRINT:PRINT"THE FUNCTIONS ARE:-"
210 PRINT:PRINT:PRINT:GOSUB 9000:PRINT:PRINT:PRINT:PRINT
220 PRINT:PRINT:PRINT"S-HORT RANGE SCAN":PRINT"L-ONG RANGE SCAN"
230 PRINT"G-ALAXY SCAN":PRINT"M-ANOUVRE":PRINT"E-NGINEERING"
240 PRINT"P-HASERS":PRINT"T-ORPEDOES":PRINT"C-OMPUTER"
245 PRINT"D-AMAGE REPORT"
250 PRINT"J-UMP DRIVE":PRINT"R-EPAIR":PRINT
260 PRINT"THESE ARE DETAILED AS FOLLOWS:-":PRINT:GOSUB9000
270 PRINT:PRINT:PRINT:PRINT"SHORT RANGE SCAN"
275 PRINT"----------------":PRINT
280 PRINT"THIS IS A DETAILED SCAN OF THE GALACTIC"
290 PRINT"SECTOR YOU ARE IN WHICH DISPLAYS WHAT IS"
300 PRINT"IN EACH LOCATION, USING THE FOLLOWING CODES:-"
310 PRINT:PRINT"E=ENTERPRISE (YOU ARE HERE!)"
320 PRINT"B=STARBASE (WHERE YOU CAN RE-FUEL)"
340 PRINT"*=A STAR (THESE JUST GET IN THE WAY)"
350 PRINTCHR$(213);"=EMPTY SPACE"
360 PRINT:PRINT"(EACH SECTOR IS 4 BY 4 'WARPS')"
380 GOSUB9000
400 PRINT"LONG RANGE SCAN":PRINT"---------------":PRINT
410 PRINT"THIS GIVES GENERAL INFORMATION ABOUT"
420 PRINT"YOUR SECTOR AND THE 8 SECTORS"
430 PRINT"SURROUNDING IT (YOURS IS THE MIDDLE ONE)"
440 PRINT:PRINT"THE 'TENS' DIGIT IS THE NO. OF KLINGONS"
450 PRINT"AND THE 'UNITS' IS THE NUMBER OF"
460 PRINT"STARBASES IN EACH SECTOR"
470 PRINT"E.G. '21' MEANS TWO KLINGINS"
480 PRINT"AND ONE STARBASE IN THAT SECTOR"
490 PRINT"(A '9' MEANS THAT SECTOR IS OUTSIDE"
500 PRINT"THE GALAXY-THEREFORE PROHIBITED)"
510 PRINT:GOSUB9000
600 PRINT"GALAXY SCAN":PRINT"-----------":PRINT
610 PRINT"THIS IS A COMPUTERIZED RECORD OF EVERYWHERE"
620 PRINT"YOU HAVE BEEN TO-USING THE SAME CODE AS"
630 PRINT"LONG RANGE SCAN, EXCEPT THAT '**'"
640 PRINT"DENOTES UNEXPLORED AREAS"
650 PRINT:PRINT"THE GALAXY SCAN IS UPDATED BY THE"
660 PRINT"SHORT RANGE OR LONG RANGE SCANS"
670 PRINT:PRINT:PRINT:GOSUB9000
700 PRINT"MANOUVRE":PRINT"--------":PRINT
710 PRINT"TO MOVE YOUR SHIP AROUND THE GALAXY"
720 PRINT"YOU MAY USE YOUR WARP ENGINES"
730 PRINT"THIS COSTS ENERGY AT A RATE OF 5"
740 PRINT"ENERGY UNITS PER 'WARP'"
750 PRINT"(REMEMBER EACH SECTOR IS 10 WARPS ACCROSS)"
752 PRINT"FIRST YOU INPUT YOUR COURSE IN O'CLOCK"
754 PRINT"I.E. 12(O'CKOCK)=UP THE SCREEN ETC."
760 PRINT"THEN YOU INPUT THE WARP SPEED(1-20)"
770 PRINT"YOU WISH TO GO AT, THEN THE TIME(1-8)"
780 PRINT"IN STARDATES YOU WISH TO TRAVEL AT"
790 PRINT"THAT SPEED FOR"
795 PRINT:GOSUB9000
800 PRINT:PRINT"IF YOU ARE GOING TO HIT SOMTHING"
810 PRINT"THE COMPUTER WILL WARN YOU AND ASK"
820 PRINT"YOU TO RE-INPUT COURSE ETC."
830 PRINT:PRINT"TO DOCK WITH A STARBASE AND REPLENISH"
840 PRINT"YOUR ENERGY AND REPAIR YOUR SHIP"
850 PRINT"(WHICH TAKES 5 STARDATES)"
860 PRINT"SIMPLY MOVE ALONGSIDE-THE COMPUTER"
870 PRINT"WILL ASK IF YOU WISH TO DOCK (ANSWER 'YES')"
880 PRINT:PRINT"IF YOUR ENGINES ARE DAMAGED IT COSTS MORE"
890 PRINT"TO MOVE - SEE 'MANOUVRE RATE' IN YOUR STATUS"
895 PRINT"REPORT TO SEE HOW MUCH.":PRINT:GOSUB9000
900 PRINT"ENGINEERING":PRINT"-----------":PRINT
910 PRINT"THIS TELLS YOU HOW MUCH ENERGY YOU HAVE LEFT"
920 PRINT"(YOU START WITH 3000 UNITS)"
930 PRINT"AND HOW MUCH HAS BEEN DIVERTED TO SHIELDS"
940 PRINT"(YOUR SHIELDS PROTECT YOU FROM KLONGON"
950 PRINT"FIRE BY ABSORBING IT-UNTIL THEY RUN OUT)"
960 PRINT"YOU MAY THEN RESET YOUR SHIELDS"
970 PRINT"OR DIVERT THEIR ENERGY IF NEEDED"
980 PRINT:PRINT:PRINT:PRINT:GOSUB 9000
1000 PRINT"PHASERS":PRINT"-------":PRINT
1010 PRINT"THIS IS WHERE YOU GET TO SHOOT AT THEM!"
1020 PRINT:PRINT"SIMPLY INPUT THE AMOUNT OF ENERGY"
1030 PRINT"YOU ARE USING-IT WILL BE DEVIDED"
1040 PRINT"EQUALLY BETWEEN THE KLINGONS IN YOUR SECTOR"
1060 PRINT"THOSE NOT DESTROYED WILL BE DAMAGED"
1070 PRINT:PRINT"EACH KLINGON CAN TAKE UP TO 50"
1080 PRINT"ENERGY UNITS OF PHASER FIRE-BUT THIS"
1090 PRINT"IS DEVIDED BY THE INTERVENING DISTANCE"
1100 PRINT"DUE TO DISPERSION":PRINT:GOSUB 9000
1110 PRINT"TORPEDOES":PRINT"---------":PRINT
1120 PRINT"YOU HAVE 8 OF THESE, WHICH COST NOTHING"
1130 PRINT"IN ENERGY TO FIRE (BUT YOU CAN ONLY"
1140 PRINT"FIRE ONE AT A TIME)"
1150 PRINT:PRINT"EACH HIT EITHER DESTROYS THE KLINGON"
1160 PRINT"OR DOES 1-100 POINTS OF DAMAGE"
1170 PRINT:PRINT:PRINT:PRINT:GOSUB9000
1200 PRINT"COMPUTER":PRINT"---------":PRINT
1210 PRINT"YOU HAVE THREE COMPUTER FUNCTIONS-"
1220 PRINT"ACCESSED BY C1, C2 OR C3"
1230 PRINT"C1 TELLS YOU THE DISTANCE AND DIRECTION"
1240 PRINT"OF EACH KLINGON IN YOUR SECTOR"
1250 PRINT"(USE THIS TO SEE WHAT BEARING YOU NEED"
1260 PRINT"TO FIRE YOUR TORPEDOES ON TO HIT)"
1270 PRINT:PRINT:PRINT:PRINT:PRINT:GOSUB9000
1300 PRINT"C2 IS A GENERAL STATUS REPORT-IT TELLS YOU"
1310 PRINT"THE NO. OF STARDATES, KLINGONS, ETC. LEFT"
1320 PRINT"AND THE MANOUVRE RATE (HOW MUCH ENERGY YOU"
1330 PRINT"NEED TO MOVE ONE WARP)"
1340 PRINT:PRINT"C3 (PHASER CALCULATION) TELLS YOU HOW"
1350 PRINT"MUCH ENERGY YOU NEED TO BE SURE TO DESTROY"
1360 PRINT"ALL THE KLINGONS IN YOUR SECTOR."
1370 PRINT:PRINT:PRINT:PRINT:PRINT:PRINT:GOSUB9000
1390 PRINT:PRINT:PRINT
1400 PRINT"DAMAGE REPORT":PRINT"-------------":PRINT
1410 PRINT"THIS TELLS YOU HOW MUCH DAMAGE EACH FUNCTION"
1420 PRINT"CAN TAKE-IF THE NO. GOES NEGATIVE THAT"
1430 PRINT"FUNCTION IS INOPERABLE"
1440 PRINT:PRINT:PRINT:PRINT:PRINT:GOSUB9000
1500 PRINT:PRINT:PRINT"JUMP DRIVE":PRINT"----------":PRINT
1510 PRINT"THIS IS AN EMERGENCY EXPERIMENTAL DEVICE"
1520 PRINT"WHICH CAUSES THE SHIP TO TRAVEL"
1530 PRINT"INSTANTANEOUSLY TO ANOTHER PART OF THE GALAXY"
1540 PRINT"-UNFORTUNATELY IT IS IMPOSSIBLE TO CALCULATE"
1550 PRINT"WHERE THE SHIP WILL END UP, UNTIL THE DEVICE"
1560 PRINT"IS USED."
1570 PRINT" IT COSTS 200 ENERGY UNITS TO USE"
1580 PRINT:PRINT:PRINT:GOSUB9000
1600 PRINT:PRINT:PRINT"REPAIR":PRINT"------"
1610 PRINT:PRINT"WHEN FUNCTIONS ARE DAMAGED THEY MAY BE"
1620 PRINT"REPAIRED INTO A JUST OPERABLE STATE"
1630 PRINT"USING THIS FUNCTION"
1640 PRINT" BUT THIS COSTS TIME AND ENERGY":PRINT:PRINT
1650 PRINT:PRINT:PRINT:GOSUB9000
1660 PRINT:PRINT:PRINT:PRINT"THIS CONCLUDES THE INSTRUCTIONS"
1670 PRINT"THE NEXT PROGRAM IS THE GAME ITSELF"
1680 PRINT"ONE LAST WORD OF WARNING"
1690 PRINT"WATCH OUT FOR BLACK HOLES AND TIME WARPS!"
1700 PRINT"AND STRONGER KLINGONS!"
1710 PRINT:PRINT"  GOOD LUCK!":PRINT:PRINT:END
9000 PRINT"(PRESS SPACE BAR TO TURN PAGE)";
9010 POKE 530,1:POKE 57088,253
9020 WAIT 57088,255,255:PRINT:PRINT:PRINT:RETURN
OK