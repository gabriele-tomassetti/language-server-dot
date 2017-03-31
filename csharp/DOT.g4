/*
 [The "BSD licence"]
 Copyright (c) 2013 Terence Parr
 All rights reserved.
*/
/**
Derived from http://www.graphviz.org/doc/info/lang.html.
Comments pulled from spec.
*/
grammar DOT;

// Parser

graph       : STRICT? (GRAPH | DIGRAPH) id? '{' stmt_list '}' ;

stmt_list   : (stmt ';'?)*  ;

stmt        : node_stmt #nodeStmt
            | edge_stmt #edgeStmt
            | attr_stmt #attrStmt
            | id '=' id #globalAttrStmt
            | subgraph  #subgraphStmt
            ;

attr_stmt   : (GRAPH | NODE | EDGE) attr_list  ;

attr_list   : ('[' a_list? ']')+ ;

a_list      : (id ( '=' id )? ','?)+ ;

edge_stmt   : (node_id | subgraph) edgeRHS attr_list? ;

edgeRHS     : (edgeop ( node_id | subgraph ))+ ;

edgeop      : '->' | '--' ;

node_stmt   : node_id attr_list? ;

node_id     : id port? ;

port        : ':' id ( ':' id )? ;

subgraph    : ( SUBGRAPH id? )? '{' stmt_list '}' ;

id          : ID | STRING | HTML_STRING | NUMBER ;

// Lexer

STRICT      : S T R I C T ;

GRAPH       : G R A P H;

DIGRAPH     : D I G R A P H ;

NODE        : N O D E ;

EDGE        : E D G E ;

SUBGRAPH    : S U B G R A P H ;

/** "a numeral [-]?(.[0-9]+ | [0-9]+(.[0-9]*)? )" */
NUMBER      : '-'? ('.' DIGIT+ | DIGIT+ ('.' DIGIT* )?) ;

STRING      : '"' ('\\"' | .)*? '"' ;

/** "Any string of alphabetic ([a-zA-Z\200-\377])
characters, underscores ('_') or digits ([0-9]),
not beginning with a digit"
 */
ID          : LETTER (LETTER | DIGIT)* ;

/** "HTML strings, angle brackets must occur
in matched pairs, and unescaped newlines are allowed."
 */
HTML_STRING : '<' (TAG | ~ [<>])* '>' ;

COMMENT     : '/*' .*? '*/' -> skip ;

LINE_COMMENT: '//' .*? '\r'? '\n' -> skip ;

/** "a '#' character is considered a line output
from a C preprocessor (e.g., # 34 to indicate
line 34 ) and discarded"
 */
PREPROC     : '#' .*? '\n' -> skip ;

WS          : [ \t\n\r]+ -> skip ;

// Fragments

fragment DIGIT  : [0-9] ;

fragment LETTER : [a-zA-Z\u0080-\u00FF] ;

fragment TAG    : '<' .*? '>' ;

fragment A : [aA];
fragment B : [bB];
fragment C : [cC];
fragment D : [dD];
fragment E : [eE];
fragment F : [fF];
fragment G : [gG];
fragment H : [hH];
fragment I : [iI];
fragment J : [jJ];
fragment K : [kK];
fragment L : [lL];
fragment M : [mM];
fragment N : [nN];
fragment O : [oO];
fragment P : [pP];
fragment Q : [qQ];
fragment R : [rR];
fragment S : [sS];
fragment T : [tT];
fragment U : [uU];
fragment V : [vV];
fragment W : [wW];
fragment X : [xX];
fragment Y : [yY];
fragment Z : [zZ];