--http://www.ibase.ru/firebird/doc/langref25-systables-procedures.html
--Описание столбцов таблицы RDB$PROCEDURES
SELECT
    RDB$PROCEDURE_NAME NAME,		--Имя хранимой процедуры.
    RDB$PROCEDURE_INPUTS INPUTS,	--Указывает количество входных параметров или их отсутствие (значение NULL).
    RDB$PROCEDURE_OUTPUTS OUTPUTS,	--Указывает количество выходных параметров или их отсутствие (значение NULL).
    RDB$DESCRIPTION DESCRIPTION,	--Произвольный текст примечания к процедуре.
    RDB$PROCEDURE_SOURCE SOURCE		--Исходный код процедуры на языке SQL.
FROM RDB$PROCEDURES;