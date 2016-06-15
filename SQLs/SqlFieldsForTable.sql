select RF.RDB$FIELD_POSITION, RF.RDB$FIELD_NAME FIELD_NAME,
       iif(RF.RDB$FIELD_NAME in (select S.RDB$FIELD_NAME
            from RDB$INDEX_SEGMENTS as S
            left join RDB$RELATION_CONSTRAINTS as RC on (RC.RDB$INDEX_NAME = S.RDB$INDEX_NAME)
            where RC.RDB$RELATION_NAME = @TABLE_NAME and
            RC.RDB$CONSTRAINT_TYPE = 'PRIMARY KEY'), 1, 0) PK_FIELD,
       case F.RDB$FIELD_TYPE
         when 7 then case F.RDB$FIELD_SUB_TYPE
                       when 0 then iif(RF.RDB$NULL_FLAG = 1, 'short', 'short?')
                       when 1 then iif(RF.RDB$NULL_FLAG = 1, 'decimal', 'decimal?')
                       when 2 then iif(RF.RDB$NULL_FLAG = 1, 'decimal', 'decimal?')
                     end
         when 8 then case F.RDB$FIELD_SUB_TYPE
                       when 0 then iif(RF.RDB$NULL_FLAG = 1, 'int', 'int?')
                       when 1 then iif(RF.RDB$NULL_FLAG = 1, 'decimal', 'decimal?')
                       when 2 then iif(RF.RDB$NULL_FLAG = 1, 'decimal', 'decimal?')
                     end
         when 9 then 'QUAD'
         when 10 then iif(RF.RDB$NULL_FLAG = 1, 'float', 'float?')
         when 12 then iif(RF.RDB$NULL_FLAG = 1, 'DateTime', 'DateTime?')
         when 13 then iif(RF.RDB$NULL_FLAG = 1, 'DateTime', 'DateTime?')
         when 14 then iif(RF.RDB$NULL_FLAG = 1, 'string', 'string')
         when 16 then case F.RDB$FIELD_SUB_TYPE
                        when 0 then iif(RF.RDB$NULL_FLAG = 1, 'long', 'long?')
                        when 1 then iif(RF.RDB$NULL_FLAG = 1, 'decimal', 'decimal?')
                        when 2 then iif(RF.RDB$NULL_FLAG = 1, 'decimal', 'decimal?')
                      end
         when 27 then iif(RF.RDB$NULL_FLAG = 1, 'decimal', 'decimal?')
         when 35 then iif(RF.RDB$NULL_FLAG = 1, 'DateTime', 'DateTime?')
         when 37 then 'string'
         when 40 then 'string'
         when 45 then iif(RF.RDB$NULL_FLAG = 1, 'byte[]', 'byte[]')
         when 261 then iif(RF.RDB$NULL_FLAG = 1, 'byte[]', 'byte[]')
         else 'RDB$FIELD_TYPE: ' || F.RDB$FIELD_TYPE || '?'
       end CS_TYPE,
       --min(I.RDB$INDEX_NAME) as "Idx",
       case F.RDB$FIELD_TYPE
         when 14 then F.RDB$FIELD_LENGTH
         when 37 then F.RDB$FIELD_LENGTH
         when 40 then F.RDB$FIELD_LENGTH
         else null
       end FIELD_LENGTH,

       iif(coalesce(RF.RDB$NULL_FLAG, 0) = 0, null, 1) FIELD_NOT_NULL,
       trim(replace(coalesce(RF.RDB$DEFAULT_SOURCE, F.RDB$DEFAULT_SOURCE), 'DEFAULT', '')) FIELD_DEFAULT,
       RF.RDB$DESCRIPTION FIELD_DESCRIPTION
from RDB$RELATION_FIELDS RF
join RDB$FIELDS F on (F.RDB$FIELD_NAME = RF.RDB$FIELD_SOURCE)
left outer join RDB$CHARACTER_SETS CH on (CH.RDB$CHARACTER_SET_ID = F.RDB$CHARACTER_SET_ID)
left outer join RDB$COLLATIONS DCO on ((DCO.RDB$COLLATION_ID = F.RDB$COLLATION_ID) and (DCO.RDB$CHARACTER_SET_ID = F.RDB$CHARACTER_SET_ID))
where (RF.RDB$RELATION_NAME = @TABLE_NAME) and
      (coalesce(RF.RDB$SYSTEM_FLAG, 0) = 0)
order by RF.RDB$FIELD_POSITION;
