# Разработка транслятора

## **Задача** 
На основании базового описания языка разработать транслятор. Базовое описание языка имеет следующий вид:

```EBNF
- <S> ::= <program>
- <program> ::= <variable_declaration><description_calculations>
- <description_calculations> ::= BEGIN <list_actions> END
- <list_actions> ::= <list_assignments>
- <list_actions> ::= <list_assignments><list_actions>
- <list_actions> ::= <list_operators>
- <list_actions> ::= <list_operators><list_actions>
- <variable_declaration> ::= VAR <list_variables> : LOGICAL ;
- <list_variables> ::= id
- <list_variables> ::= id , <list_variables>
- <list_operators> ::= <operator>
- <list_operators> ::= <operator><list_operators>
- <operator> ::= READ ( <list_variables> ) ;
- <operator> ::= WRITE ( <list_variables> ) ;
- <operator> ::= IF ( <expr> ) THEN <description_calculations> ELSE <description_calculations> END_IF
- <list_assignments> ::= <assignment>
- <list_assignments> ::= <assignment><list_assignments>
- <assignment> ::= id = <expr> ;
- <expr> ::= <unary_op><sub_expr>
- <expr> ::= <sub_expr>
- <sub_expr> ::= ( <expr> )
- <sub_expr> ::= <operand>
- <sub_expr> ::= <sub_expr><bin_op><sub_expr>
- <unary_op> ::= NOT
- <bin_op> ::= AND
- <bin_op> ::= OR
- <bin_op> ::= EQU
- <operand> ::= id
- <operand> ::= const
```

<br/>

**id** - Идентификатор переменной, который состоит только из латинских букв в нижнем регистре, его длина не может превосходить 11 символов.  
**const** - Число 0 или 1 (false/true)