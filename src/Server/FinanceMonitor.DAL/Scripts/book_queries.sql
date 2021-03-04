use TSQLV5


Select custId, orderdate, orderid, val,
      val - first_value(val) over(partition by custid
           order by orderdate, orderid
           rows between unbounded preceding and current row) as val_firstorder,
       val -last_value(val) over(partition by custid
           order by orderdate, orderid
           rows between current row and unbounded following) as val_lastorder
       
       from Sales.OrderValues
order by custid, orderdate, orderid;

go

with ordersRN as 
    (
        select custid, val,
               row_number() over (partition by custid
                   order by orderdate, orderid) as rna,
               row_number() over (partition by custid
                   order by orderdate desc, orderid desc) as rnd
        from sales.OrderValues
    ),
     Agg as(
         select custid,
                max(case when rna = 1 then val end) as firstorderval,
                max(case when rnd =1 then val end) as lastrorderval,
                max(case when rna = 3 then val end) as thirorderval
         from ordersRN
         Group by custid
     )
select O.custid, O.orderdate, O.orderid, O.val, A.firstorderval, A.lastrorderval, A.thirorderval
from Sales.OrderValues as O 
inner join Agg as A on O.custid = A.custid
order by custid, orderdate, orderid;

go
drop table if exists dbo.T1
create table dbo.T1(
    id int not null constraint PK_T1 Primary key,
    col1 int null
);

insert into dbo.T1(id, col1) values 
(2, null),
                                    (3, 10),
                                    (5, -1),
                                    (7, null),
                                    (11, null),
                                    (13, -12),
                                    (17, null),
                                    (19, null),
                                    (23, 1759);


with C as (
    select id,
           col1,
           max(case when col1 is not null then id end) over (order by id rows unbounded preceding ) as grp
    from dbo.T1
)
select id, col1, max(col1) over (partition by grp order by id rows unbounded preceding)
from C
go
declare @val as numeric(12,2) = 1000.00;

select custid,
       count(distinct case when val < @val then val end) + 1 as rnk
from Sales.OrderValues
group by custid
go
declare @score as tinyint  = 80;

with C as (
    select testid,
           count(case when score < @score then 1 end) + 1 as rnk,
           count(*) +1 as nr
        from Stats.Scores
    group by testid
)
select testid, 1.0 * (rnk - 1) / (nr-1) as pctrank
from C
go
declare @score as tinyint  = 80;

         with C as (
    select testid,
           count(case when score <= @score then 1 end) + 1 as rnk,
           count(*) +1 as nr
        from Stats.Scores
    group by testid
)
select testid, 1.0 * (rnk) / (nr) as pctrank
from C
go

declare @pct as float = 0.5;

select distinct testid,
                percentile_disc(@pct) within group (order by score)
over(partition by testid) as percentiledisc,
                percentile_cont(@pct) within group (order by score)
over(partition by testid) as percentilecont
from Stats.Scores
go


declare @pct as float = 0.5;
with c1 as (
    select testid, score, 
           row_number() over(partition by testid order by score) - 1 as rownum,
           @pct * (count(*) over(partition by testid) - 1) as a
    from Stats.Scores
),
     C2 as (
         select testid, score, a-Floor(a) as factor
         from C1
         where rownum in (floor(a), ceiling(a))
     )
select testid, min(score) + factor * (max(score) - min(score)) as percentilecont
from C2
group by testid, factor;

go

with C as (
    select custid,
           first_value(val) over(partition by custid
               order by orderdate, orderid
               rows between unbounded preceding and current row) as val_firstorder,
           last_value(val) over (partition by custid
               order by orderdate, orderid
               rows between current row and unbounded following) as val_lastorder,
           row_number() over(partition by custid order by (select null)) as rownum
    from Sales.OrderValues
)
select custid, val_firstorder, val_lastorder
from C
where rownum = 1

go

with C as (
    select custid,
           convert(char(8), orderdate, 112)
               + str(orderid, 10)
               + str(val, 14, 2)
               collate Latin1_General_BIN2 as s
    from Sales.OrderValues
)
select custid,
       cast(substring(min(s), 19, 14) as numeric(12,2)) as firstorderval,
       cast(substring(max(s), 19, 14) as numeric(12,2)) as lastorderval
from C
group by custid;

go

select custid,
       String_Agg(orderid, ',') within group (order by orderid) as custorders
from Sales.Orders
group by custid

Select @@version


go

drop table if exists dbo.Transactions;
drop table if exists dbo.Accounts;

create table dbo.Accounts(
    actid int not null,
    actname varchar(50) not null,
    constraint PK_Accounts primary key (actid)
);

create table dbo.Transactions(
    actid int not null,
    tranid int not null,
    val money not null,
    constraint PK_Transactions Primary key(actid, tranid),
    constraint FK_TransactionsAccounts foreign key (actid) references dbo.Accounts(actid)
);

INSERT INTO dbo.Accounts(actid, actname) VALUES
(1,  'account 1'),
(2,  'account 2'),
(3,  'account 3');

INSERT INTO dbo.Transactions(actid, tranid, val) VALUES
(1,  1,  4.00),
(1,  2, -2.00),
(1,  3,  5.00),
(1,  4,  2.00),
(1,  5,  1.00),
(1,  6,  3.00),
(1,  7, -4.00),
(1,  8, -1.00),
(1,  9, -2.00),
(1, 10, -3.00),
(2,  1,  2.00),
(2,  2,  1.00),
(2,  3,  5.00),
(2,  4,  1.00),
(2,  5, -5.00),
(2,  6,  4.00),
(2,  7,  2.00),
(2,  8, -4.00),
(2,  9, -5.00),
(2, 10,  4.00),
(3,  1, -3.00),
(3,  2,  3.00),
(3,  3, -2.00),
(3,  4,  1.00),
(3,  5,  4.00),
(3,  6, -1.00),
(3,  7,  5.00),
(3,  8,  3.00),
(3,  9,  5.00),
(3, 10, -3.00);

DROP FUNCTION IF EXISTS dbo.GetNums;
GO
CREATE OR ALTER FUNCTION dbo.GetNums(@low AS BIGINT, @high AS BIGINT) RETURNS
    TABLE
        AS
        RETURN
        WITH
            L0   AS (SELECT c FROM (VALUES(1),(1)) AS D(c)),
            L1   AS (SELECT 1 AS c FROM L0 AS A CROSS JOIN L0 AS B),
            L2   AS (SELECT 1 AS c FROM L1 AS A CROSS JOIN L1 AS B),
            L3   AS (SELECT 1 AS c FROM L2 AS A CROSS JOIN L2 AS B),
            L4   AS (SELECT 1 AS c FROM L3 AS A CROSS JOIN L3 AS B),
            L5   AS (SELECT 1 AS c FROM L4 AS A CROSS JOIN L4 AS B),
            Nums AS (SELECT ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS rownum
                     FROM L5)
        SELECT TOP (@high - @low + 1) @low + rownum - 1 AS n
        FROM Nums
        ORDER BY rownum;
go

declare @num_partitions as int = 100,
    @rows_per_partition as int = 20000;

truncate table dbo.Transactions;
delete from dbo.Accounts;
insert into dbo.Accounts with (TABLOCK )(actid, actname)
select n as actid, 'account ' + cast (n as varchar(10)) as actname
from dbo.GetNums(1, @num_partitions) as P;

insert into dbo.Transactions with (TABLOCK )(actid, tranid, val)
select NP.n, RPP.n,
       (ABS(Checksum(newID())%2)*2 -1) * (1+ ABS(checksum(newid())%5))
from dbo.GetNums(1, @num_partitions) as NP
cross join dbo.GetNums(1, @rows_per_partition) as RPP;

go

select actid, tranid, val,
       row_number() over(partition  by actid order by val) as rownum
from dbo.Transactions

    go
drop index idx_actid_val_i_tranid on dbo.Transactions

create index idx_actid_val_i_tranid
on dbo.Transactions(actid, val)
include(tranid)

go

drop table if exists dbo.Credits;
drop table if exists dbo.Debits;

select *
into dbo.Credits
from dbo.Transactions
where val > 0;

alter table dbo.Credits
add constraint PK_Credits
primary key (actid, tranid);

select *
into dbo.Debits
from dbo.Transactions
where val < 0;

alter table dbo.Debits
add constraint PK_debits primary key (actid, tranid);
go

create index idx_actid_val_i_tranid
on dbo.Credits(actid, val)
include (tranid);

create index idx_actid_val_i_tranid
on dbo.Debits(actid, val)
include (tranid);

go

with C as(
    select actid, tranid, val from dbo.Debits
    union all
    select actid, tranid, val from dbo.Credits
)
select actid, tranid, val,
       row_number() over (partition by actid order by val) as rownum
from C;

go
alter database TSQLV5 set compatibility_level  = 140
go
select actid, tranid, val,
       row_number() over(order by actid desc, val desc) as rownum
from dbo.Transactions
where tranid < 1000;

go

select actid, tranid, val,
       row_number() over(partition by actid order by val desc) as rownum
from dbo.Transactions
order by actid desc;

with C as (
    select orderid, shippeddate, 1 as sortcal
    from Sales.Orders
    where shippeddate is not null
    union all
    select orderid, shippeddate, 2 as sortcal
    from Sales.Orders
    where shippeddate is null
)
select orderid, shippeddate,
       row_number()  over (order by sortcal, shippeddate) as rownum
from C;

select actid, tranid, val,
       row_number() over (partition by actid order by val) as rownumasc,
       row_number() over (partition by actid order by val desc) as rownumdesc
from dbo.Transactions;

go
select C.actid, A.tranid, A.val, A.rownumasc, A.rownumdesc
from dbo.Accounts as C
cross apply (select tranid, val,
                    row_number() over (order by val) as rownumasc,
                    row_number() over (order by val desc) as rownumdesc
    from dbo.Transactions as T
    where T.actid = C.actid) as A
go
drop index if exists idx_actid_val_i_tranid on dbo.Transactions





go
select actid, tranid, val,
       row_number() over (partition by actid order by val) as rownum
from dbo.Transactions
OPTION (RECOMPILE);
go
create nonclustered columnstore index idx_cs
on dbo.Transactions(actid, tranid, val);
go
drop index if exists idx_cs on dbo.Transactions
create index idx_actid_val_i_tranid
on dbo.Transactions(actid, val)
include (tranid);
create nonclustered columnstore index idx_cs on dbo.Transactions(actid)
where actid = -1 and actid = -2
go



drop index if exists idx_cs on dbo.Transactions
go
select actid, tranid, val,
       sum(val) over (partition by actid) as balance
from dbo.Transactions
go


alter database TSQLV5 set compatibility_level  = 150;

go
select actid, tranid, val,
       row_number() over (order by (select null)) as rownum
from dbo.Transactions;

go
select actid, tranid, val,
       ntile(100) over (partition by actid order by val) as ronum
from dbo.Transactions
go

with C as (
    select actid,
           tranid,
           val,
           max(val) over (partition by actid) as mx
    from dbo.Transactions
)
select actid, tranid, val
from C
where val = mx
--option (maxdop 1);

alter database TSQLV5 set compatibility_level = 140

go

select actid, tranid, val,
       max(val) over(partition  by actid
order by tranid
rows between 10000 preceding 
and 10000 preceding ) as sumval
from dbo.Transactions;

set statistics io on;
create event session xe_window_spool on server
add event sqlserver.window_spool_ondisk_warning
(action (sqlserver.plan_handle, sqlserver.sql_text));

drop event session xe_window_spool on server

alter event session xe_window_spool on server state = start;


