10 FORI=1TO16:PRINT:NEXT:PRINT"STARTREK INSTRUCTIONS (PART 2)
20 PRINT"------------------------------":PRINT
30 PRINT"COMPUTER FUNCTION":PRINT"-----------------":PRINT
40 PRINT"THERE ARE THREE COMPUTER FUNCTIONS"
50 PRINT"SO TYPE C1, C2 OR C3 TO USE EACH ONE"
60 PRINT:PRINT"C1 IS A KLINGON SCAN-YOU GET THE"
70 PRINT"DISTANCE AND DIRECTION (IN O'CLOCK)"
80 PRINT"OF EACH KLINGON IN YOUR SECTOR"
85 PRINT:GOSUB 9000:PRINT:PRINT
90 PRINT"C2 IS A STATUS REPORT (NO. OF KLINGONS"
100 PRINT"LEFT ETC.)"
115 PRINT:PRINT:PRINT"C3 IS PHASAR CALCULATION"
120 PRINT"I.E. HOW MUCH ENERGY YOU MUST USE TO"
130 PRINT"DESTROY ALL THE KLINGONS IN YOUR SECTOR"
140 PRINT:PRINT:PRINT:GOSUB 9000
150 PRINT"DAMAGE REPORT":PRINT"-------------":PRINT
160 PRINT"THIS GIVES A LIST OF EACH FUNCTION"
170 PRINT"AND THE AMOUNT OF DAMAGE IT CAN TAKE"
180 PRINT"(NEGATIVE NOS. MEAN THAT FUNCTION IS"
190 PRINT"INOPERABLE). DAMAGE MAY BE REPAIRED"
200 PRINT"BY DOCKING WITH A STARBASE."
210 PRINT:PRINT:PRINT:PRINT:GOSUB 9000
220 PRINT"JUMP DRIVE":PRINT"----------":PRINT
230 PRINT"THIS IS AN EMERGENCY DEVICE FOR USE"
240 PRINT"IF YOUR ENGINES ARE BADLY DAMAGED"
250 PRINT"(I.E. MANOUVRE RATE VERY HIGH)"
260 PRINT"IT COSTS 200 ENERGY UNITS TO OPERATE"
270 PRINT"AND CAUSES YOUR SHIP TO DISAPEAR"
280 PRINT"AND RE-APPEAR AT RANDOM SOMWHERE"
290 PRINT"IN THE GALAXY.":PRINT:PRINT
300 GOSUB9000:PRINT:PRINT
310 PRINT"THAT COMPLETES THE INSTRUCTIONS"
320 PRINT"THE NEXT PROGRAM IS THE GAME ITSELF"
330 PRINT:PRINT"     GOOD LUCK!":PRINT:PRINT:PRINT:END
9000 PRINT"(PRESS SPACE BAR TO TURN PAGE)";
9010 POKE 530,1:POKE 57088,253:WAIT 57088,255,255
9020 PRINT:PRINT:PRINT:PRINT:PRINT:RETURN
OK