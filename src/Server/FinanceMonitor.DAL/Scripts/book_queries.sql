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


----------------
go
use TSQLV5;
go
create or alter function dbo.GetNums(@low as bigint, @high as bigint) returns
    table
    as 
    return 
with 
     L0 as (select c from (values(1),(1)) as D(c)),
     L1 as (select 1 as c from L0 as a cross join L0 as B),
     L2 as (select 1 as c from L1 as a cross join L1 as B),
     L3 as (select 1 as c from L2 as a cross join L2 as B),
     L4 as (select 1 as c from L3 as a cross join L3 as B),
     L5 as (select 1 as c from L4 as a cross join L4 as B),
     Nums as (select row_number() over (order by (select null)) as rownum from L5)
select top (@high - @low + 1) @low + rownum - 1 as n
from Nums
order by rownum;
go

select n from dbo.GetNums(11, 20)
go
declare 
    @start as DAte = '20190201',
    @end as Date = '20190212';

select Dateadd(day, n, @start) as dt
from dbo.GetNums(0, Datediff(day, @start, @end)) as Nums;

go
drop table if exists Sales.MyOrders;
go
select 0 as orderid, custid, empid, orderdate
into Sales.MyOrders
from Sales.Orders;

select * from Sales.MyOrders
go
with c as (
    select orderid, row_number() over (order by orderdate, custid) as rownum
    from Sales.MyOrders
)
update c
set orderid = rownum;
go
drop table if exists dbo.MySequence;
create table dbo.MySequence(val int);
insert into dbo.MySequence values(0);
go
create or alter procedure dbo.GetSequence
@val as int output,
@n as int = 1
as
    begin 
        update MySequence
        set @val = val + 1,
            val += @n;
    end
go

declare @key as int;
exec dbo.GetSequence @val = @key output;
select @key

go
select custid
from Sales.Customers
where country = N'UK';

go
declare @firstkey as int, @rc as int;
declare @CustsStage as table(
    custid int,
    rownum int
                            );

insert into @CustsStage(custid, rownum)
select custid, row_number() over (order by (select null)) as rownum
from sales.Customers
where country = N'UK'

set @rc = @@rowcount;

exec dbo.GetSequence @val = @firstkey output, @n = @rc;

select custid, @firstkey + rownum - 1 as keycol
from @CustsStage

select * from MySequence
go
drop procedure if exists dbo.GetSequence;
drop table if exists dbo.MySequence;

go
create unique index idx_od_oid_i_cid_eid
on Sales.Orders(orderdate, orderid)
include (custid, empid);
go
declare @pagenum as int = 3,
    @pagesize as int = 25;

with c as (
    select row_number() over (order by orderdate, orderid) as rownum,
           orderid, orderdate, custid, empid
    from Sales.Orders
)
select orderid, orderdate, custid, empid
from c
where rownum between (@pagenum -1) * @pagesize + 1 and @pagenum * @pagesize
order by rownum;
go;

drop table if exists Sales.MyOrders;
go

select * into Sales.MyOrders from Sales.Orders
Union all
select * from Sales.Orders where orderid % 100 = 0
union all
select * from Sales.Orders where orderid % 50 = 0;

go

select orderid, row_number() over (partition by orderid order by (select null)) as n 
from Sales.MyOrders;
go
with c as
    (
        select orderid, row_number() over (partition by orderid order by (select null)) as n
        from sales.MyOrders
    )
delete from C where n> 1;
go

with c as
         (
             select *, row_number() over (partition by orderid order by (select null)) as n
             from sales.MyOrders
         )
select orderid, custid, empid, orderdate, requireddate, shippeddate,
       shipperid, freight, shipname, shipaddress, shipcity, shipregion,
       shippostalcode, shipcountry
into Sales.MyOrdersTmp
from C
where n = 1;

-- re-create indexes, constraints

truncate table Sales.MyORders;
alter table Sales.MyOrdersTmp switch to Sales.MyOrders;
Drop table Sales.MyOrdersTmp;

go

select orderid,
       row_number() over(order by orderid) as rownum,
       rank() over(order by orderid) as rnk
from Sales.MyOrders
go
drop table if exists Sales.MyOrders
go

with C as (
    select Year(orderdate) as orderyear, Month(orderdate) as ordermonth, val
    from Sales.OrderValues
)
select *
from C
Pivot(sum(val)
for ordermonth in ([1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12])) as P;
go
with C as (
    select custid,
           cast(orderid as varchar(11)) as sorderid,
           row_number() over (partition by custid order by orderdate desc, orderid desc) as rownum
    from Sales.OrderValues
)
select custid, concat([1], ',' + [2],',' + [3],','+ [4],','+ [5]) as orderids
from C
pivot (max(sorderid) for rownum in ([1],[2],[3],[4],[5])) as P;
go

------
create unique index idx_cid_odD_oidD_i_empid
on Sales.Orders(custid, orderdate desc, orderid desc)
include (empid)
go
with C as (
    select custid, orderdate, orderid, empid, 
    row_number()  over (partition by custid order by orderdate desc, orderid desc) as rn
    from Sales.Orders
    )
select custid, orderdate, orderid, empid, rn
from C
where rn <= 3
order by custid, rn;

go

select C.custid, A.orderdate, A.orderid, A.empid
from Sales.Customers as C
cross apply (select top (3) orderdate, orderid, empid
    from Sales.Orders as O
    where O.custid = C.custid
    order by orderdate desc, orderid desc) as A
go
drop index if exists idx_cid_odD_oidD_i_empid on Sales.Orders
go

----

drop table if exists dbo.T1;
go

create table dbo.T1(
    id int not null constraint PK_T1 primary key,
    col1 int null
)

insert into dbo.T1(id, col1) values
(2, null),
(3, 10),
                                    (5, -1),
                                    (7, null),
                                    (11, null),
                                    (13, -12),
                                    (17, null),
                                    (19, null),
                                    (23, 1759)
                                    
                                    go

with C as(
    select id, col1, 
           max(case when col1 is not null then id end)
            over (order by id rows unbounded preceding ) as grp
    from T1
)
select id, col1,
       max(col1) over (partition by grp order by id
           rows unbounded  preceding ) as lastValue
from C
option(use hint('DISALLOW_BATCH_MODE'))
go
truncate table T1
select n as id, checksum(newid()) as col1
from dbo.GetNums(1, 10000000) as Nums
option(maxdop 1);
go
select id, col1, binstr,
        max(binstr) over (order by id rows unbounded preceding) as mx
from T1
         cross apply (values(cast(id as binary(4)) + cast(col1 as binary(4)))) as A(binstr);

go
select id, col1, cast(substring(max(cast(id as binary(4)) + cast(col1 as binary(4))) over (order by id rows unbounded preceding), 5,4) as int) as lastval
from T1

;
go
create index idx_custid_empid on Sales.Orders(custid, empid);
go
with C as (
    select custid,
           empid,
           count(*)                                                                   as cnt,
           rank() over (partition by custid order by count(*) desc) as rn
    from Sales.Orders
    group by custid, empid
)
select * from C
where rn = 1
go
drop index if exists idx_custid_empid on Sales.Orders
go
with C as(
select empid, val,
       ntile(20) over (partition by empid order by val) as ntile20
from Sales.OrderValues)
select empid, avg(val) as avgval
from C
where ntile20 between 2 and 19
group by empid;
go
drop table if exists Transactions
create table Transactions
(
    actid int not null,
    tranid int not null,
    val money not null,
    constraint PK_Transactions Primary Key (actid, tranid)
);
go

insert into Transactions(actid, tranid, val) values
(1, 1, 4.00),
(1, 2, -2.00),
(1, 3, 5.00),
(1, 4, 2.00),
(1, 5, 1.00),
(1, 6, 3.00),
(1, 7, -4.00),
(1, 8, -1.00),
(1, 9, -2.00),
(1, 10, -3.00),
(2, 1, 2.00),
(2, 2, 1.00),
(2, 3, 5.00),
(2, 4, 1.00),
(2, 5, -5.00),
(2, 6, 4.00),
(2, 7, 2.00),
(2, 8, -4.00),
(2, 9, -5.00),
(2, 10, 4.00),
(3, 1, -3.00),
(3, 2, 3.00),
(3, 3, -2.00),
(3, 4, 1.00),
(3, 5, 4.00),
(3, 6, -1.00),
(3, 7, 5.00),
(3, 8, 3.00),
(3, 9, 5.00),
(3, 10, -3.00);
go
select * from Transactions
go
create index idx_actid  on Transactions (actid, tranid) include (val)
drop index if exists idx_actid on Transactions
go
declare @num_partitions  as int = 100,
        @rows_per_partition as int = 10000;

truncate table Transactions;
insert into Transactions with (tablock) (actid, tranid, val)
select NP.n, RPP.n,
       (ABS(checksum(newid())%2)*2-1) * (1 + abs(checksum(newid())%5))
from GetNums(1, @num_partitions) as NP
cross join GetNums(1, @rows_per_partition) as rpp;
go
select actid, tranid, val,
       sum(val) over (partition by actid 
                        order by tranid 
                        rows unbounded preceding) as balance
from Transactions;

select actid, tranid, val,
       (select sum(t2.val) from Transactions as T2
           where T2.actid = T1.actid and 
                 T2.tranid <= T1.tranid) as balance
from Transactions as T1;

select T1.actid, T1.tranid, T1.val,
       sum(T2.val) as balance
from Transactions as T1
inner join Transactions T2 on T1.actid = T2.actid 
and T2.tranid   <= T1.tranid
group by T1.actid, T1.tranid, T1.val;

go
declare @Result as table
(
    actid int,
    tranid int,
    val money,
    balance money
);

declare 
    @C As Cursor,
    @actid as int,
    @prvactid as int,
    @tranid as int,
    @val as money,
    @balance as money;

set @C = cursor forward_only static read_only for
select actid, tranid, val
from Transactions
order by actid, tranid;

open @C
fetch next from @c into @actid, @tranid, @val;

select @prvactid = @actid, @balance = 0;

while @@fetch_status = 0 
begin
    if @actid <> @prvactid
        select @prvactid = @actid, @balance = 0;
        
    set @balance = @balance + @val;
    
    insert into @Result values(@actid, @tranid, @val, @balance);
    
    fetcH next from @C into @actid, @tranid, @val;
end

select * from @Result;
go
select actid, tranid, val,
       row_number() over(partition by actid order by tranid) as rownum
into #Transactions
from Transactions

create unique clustered index idx_rownum_actid on #Transactions(rownum, actid);

with C as(
    select 1 as rownum, actid, tranid, val, val as sumqty
    from #Transactions
    where rownum = 1
    
    union all
    select prv.rownum + 1, prv.actid, prv.tranid, cur.val, prv.sumqty + cur.val
    from C as prv
    inner join #Transactions as cur
    on cur.rownum = prv.rownum+1
    and cur.actid = prv.actid
)
select actid, tranid, val, sumqty
from c
option (maxrecursion 0);
go

drop table if exists dbo.Sessions;
create table dbo.Sessions
(
    keycol    int          not null,
    app       varchar(10)  not null,
    usr       varchar(10)  not null,
    host      varchar(10)  not null,
    starttime datetime2(0) not null,
    endtime   datetime2(0) not null,
    constraint PK_sessions primary key (keycol),
    check (endtime > starttime)
);
go

TRUNCATE TABLE dbo.Sessions;
INSERT
INTO dbo.Sessions(keycol,
                  app, usr,
                  host, starttime, endtime)
VALUES (2, 'app1', 'user1', 'host1', '20190212 08:30', '20190212 10:30'),
       (3,
        'app1', 'user2', 'host1',
        '20190212 08:30', '20190212 08:45'),
       (5,
        'app1', 'user3', 'host2',
        '20190212 09:00', '20190212 09:30'),
       (7,
        'app1', 'user4', 'host2',
        '20190212 09:15', '20190212 10:30'),
       (11,
        'app1', 'user5', 'host3',
        '20190212 09:15', '20190212 09:30'),
       (13,
        'app1', 'user6', 'host3',
        '20190212 10:30', '20190212 14:30'),
       (17,
        'app1', 'user7', 'host4',
        '20190212 10:45', '20190212 11:30'),
       (19,
        'app1', 'user8', 'host4',
        '20190212 11:00', '20190212 12:30'),
       (23,
        'app2', 'user8', 'host1',
        '20190212 08:30', '20190212 08:45'),
       (29,
        'app2', 'user7', 'host1',
        '20190212 09:00', '20190212 09:30'),
       (31,
        'app2', 'user6', 'host2',
        '20190212 11:45', '20190212 12:00'),
       (37,
        'app2', 'user5', 'host2',
        '20190212 12:30', '20190212 14:00'),
       (41,
        'app2', 'user4', 'host3',
        '20190212 12:45', '20190212 13:30'),
       (43,
        'app2', 'user3', 'host3',
        '20190212 13:00', '20190212 14:00'),
       (47,
        'app2', 'user2', 'host4',
        '20190212 14:00', '20190212 16:30'),
       (53,
        'app2', 'user1', 'host4',
        '20190212 15:30', '20190212 17:00');
;
go
truncate table dbo.Sessions

declare
    @numrows as int = 1000000,
    @numapps as int = 10;

insert into dbo.Sessions with (tablock)
    (keycol, app, usr, host, starttime, endtime)
select row_number() over (order by (select null)) as keycol,
       D.*,
       Dateadd(second,
               1 + abs(checksum(newid())) % (20 * 60),
               starttime)                         as endtime
from (
         select 'app' + cast(1 + abs(checksum(newid())) % @numapps as varchar(10)) as app,
                'users1'                                                           as usr,
                'host1'                                                            as host,
                dateadd(
                        second,
                        1 + abs(checksum(newid())) % (30 * 24 * 60 * 60),
                        '20190101')                                                as starttime
         from dbo.GetNums(1, @numrows) as nums
     )
         as D;
go
create index idx_start_end on dbo.SEssions (app, starttime, endtime)
go

with TimePoints as (
    select app, starttime as ts
    from dbo.Sessions
),
     Counts as (
         select app,
                ts,
                (select count(*)
                 from dbo.Sessions as S
                 where P.app = S.app
                   and P.ts >= S.starttime
                   and P.ts < S.endtime) as concurrent
         From TimePoints as P
     )
select app, max(concurrent) as mx
from Counts
Group by app;
go
drop index if exists idx_start_end on dbo.Sessions
go
-----
create unique index idx_start on dbo.Sessions (app, starttime, keycol);
create unique index idx_end on dbo.Sessions (app, endtime, keycol);
go
with C1 as (
    select keycol, app, starttime as ts, +1 as type
    from dbo.Sessions
    union all
    select keycol, app, endtime as ts, -1 as type
    from dbo.Sessions
),
     C2 as (
         select *,
                sum(type) over (partition by app
                    order by ts, type,keycol
                    rows unbounded preceding) as cnt
         from C1
     )
select app, max(cnt) as mx
from C2
group by app
option (maxdop 1);
go
-----

drop table if exists dbo.Sessions, dbo.Users;

create table Users
(
    username varchar(14) not null,
    constraint PK_Users primary key (username)
);

create table Sessions
(
    id        int          not null identity (1,1),
    username  varchar(14)  not null,
    starttime datetime2(3) not null,
    endtime   datetime2(3) not null,
    constraint PK_Sessions primary key (id),
    constraint CHK_endtime_gteq_starttime
        check (endtime >= starttime)
);
go
declare
    @num_users as int = 2000,
    @intervals_per_user as int = 2500,
    @start_perdiod as datetime2(3) = '20190101',
    @end_period as datetime2(3) = '20190107',
    @max_duration_in_ms as int = 3600000;

truncate table Sessions;
truncate table Users;

insert into Users(username)
select 'User' + right('000000000' + cast(U.n as varchar(10)), 10) as username
from dbo.GetNums(1, @num_users) as U;

with C as (
    select 'User' + Right('000000000' + cast(U.n as varchar(10)), 10)                            as username,
           dateadd(ms, abs(checksum(newid())) % 86400000,
                   dateadd(day, abs(checksum(newid())) % datediff(day, @start_perdiod,
                                                                  @end_period), @start_perdiod)) as starttime
    from dbo.GetNums(1, @num_users) as U
             cross join dbo.GetNums(1, @intervals_per_user) as I
)
insert
into dbo.Sessions with (tablock) (username, starttime, endtime)
select username,
       starttime,
       dateadd(ms, abs(checksum(newid())) % (@max_duration_in_ms + 1), starttime) as endtime
from C;
go
create nonclustered columnstore index idx_cs on dbo.Sessions (id)
    where id = -1 and id = -2;
create unique index idx_user_start_id on dbo.Sessions (username, starttime, id);
create unique index idx_user_end_id on dbo.Sessions (username, endtime, id);
go
with C1 as (
    select id, username, starttime as ts, + 1 as type
    from dbo.Sessions
    union all
    select id, username, endtime as ts, -1 as type
    from dbo.Sessions
),
     C2 as (
         select username,
                ts,
                type,
                sum(type) over (partition by username order by ts, type desc, id
                    rows unbounded preceding) as cnt
         from C1
     ),
     C3 as (
         select username,
                ts,
                (row_number() over (partition by username order by ts) - 1) / 2 + 1 as grp
         from C2
         where (type = 1 and cnt = 1)
            or (type = -1 and cnt = 0)
     )
select username, min(ts) as starttime, max(ts) as endtime
from C3
group by username, grp;
go
drop index if exists idx_user_start_id on dbo.Sessions;
drop index if exists idx_user_end_id on dbo.Sessions;
go
create unique index idx_user_start_end_id
    on dbo.Sessions (username, starttime, endtime, id);
go
with C1 as (
    select *,
           case
               when starttime <= max(endtime) over (partition by username
                   order by starttime, endtime, id
                   rows between unbounded preceding and 1 preceding)
                   then 0
               else 1
               end as isstart
    from dbo.Sessions
),
     C2 as (
         select *,
                sum(isstart) over (partition by username
                    order by starttime, endtime, id
                    rows unbounded preceding) as grp
         from C1
     )
select username, min(starttime) as starttime, max(endtime) as endtime
from C2
group by username, grp;
go

drop table if exists dbo.T1;
create table dbo.T1
(
    col1 int not null
        constraint PK_T1 primary key
);
go
insert into dbo.T1(col1)
values (2),
       (3),
       (7),
       (8),
       (9),
       (11),
       (15),
       (16),
       (17),
       (28);

drop table if exists dbo.T2;
create table dbo.T2
(
    col1 date not null
        constraint PK_T2 primary key
);
go
insert into dbo.T2(col1)
values ('20190202'),
       ('20190203'),
       ('20190207'),
       ('20190208'),
       ('20190209'),
       ('20190211'),
       ('20190215'),
       ('20190216'),
       ('20190217'),
       ('20190228');
go
create nonclustered columnstore index idx_cs on dbo.T1 (col1)
    where col1 = -1 and col1 = -2;
create nonclustered columnstore index idx_cs on dbo.T2 (col1)
    where col1 = '00010101' and col1 = '00010102';
go
with C as (
    select col1 as cur, lead(col1) over (order by col1) as nxt
    from dbo.T1
)
select cur + 1 as rangestart, nxt - 1 as rangeend
from C
where nxt - cur > 1;
go
with C as (
    select col1 as cur, lead(col1) over (order by col1) as nxt
    from dbo.T2
)
select dateadd(day, 1, cur) as rangestart, dateadd(day, -1, nxt) rangeend
from C
where datediff(day, cur, nxt) > 1;
go
with C as (
    select col1,
           col1 - dense_rank() over (order by col1) as grp
    from dbo.T1
)
select min(col1) as start_range, max(col1) as end_range
from C
group by grp;

with C as (
    select col1, dateadd(day, -1 * dense_rank() over (order by col1), col1) as grp
    from dbo.T2
)
select min(col1) as start_range, max(col1) as end_range
from C
group by grp
go
with C1 as (
    select col1,
           case
               when datediff(day, lag(col1) over (order by col1), col1) <= 2
                   then 0
               else 1
               end as isstart
    from dbo.T2
),
     C2 as (
         select *,
                sum(isstart) over (order by col1 rows unbounded preceding) as grp
         from C1
     )
select Min(col1) as rangestart, max(col1) as rangeed
from C2
group by grp;
go

select distinct testid,
                percentile_cont(0.5) within group (order by score) over (partition by testid)                           as median,
                avg(score)
                    over (partition by testid order by score rows between unbounded preceding and UNBOUNDED FOLLOWING ) as avg
from Stats.Scores;
go

drop table if exists dbo.T1;
go
create table dbo.T1
(
    ordcol  int not null primary key,
    datacol int not null
);

insert into dbo.T1
values (1, 10),
       (4, -15),
       (5, 5),
       (6, -10),
       (8, -15),
       (10, 20),
       (17, 10),
       (18, -10),
       (20, -30),
       (31, 20);

with C1 as (
    select ordcol,
           datacol,
           sum(datacol) over (order by ordcol
               rows unbounded preceding) as partsum
    from dbo.T1
),
     C2 as (
         select *,
                min(partsum) over (order by ordcol
                    rows unbounded preceding) as mn
         from C1
     )
select ordcol,
       datacol,
       partsum,
       adjust,
       partsum + adjust                                  as nonnegativesum,
       adjust - lag(adjust, 1, 0) over (order by ordcol) as replenish
from C2
         cross apply(values (case when mn < 0 then -mn else 0 end)) as A(adjust)

go

drop table if exists dbo.Employees;
go
create table dbo.Employees
(
    empid   int         not null
        constraint PK_Employees primary key,
    mgrid   int         null
        constraint FK_Employees_mgr_emp references dbo.Employees,
    empname varchar(25) not null,
    salary  money       not null,
    check (empid <> mgrid)
);

insert into dbo.Employees(empid, mgrid, empname, salary)
values (1, null, 'David', $10000.00),
       (2, 1, 'Eitan', $7000.00),
       (3, 1, 'Ina', $7500.00),
       (4, 2, 'Seraph', $5000.00),
       (5, 2, 'Jiru', $5500.00),
       (6, 2, 'Steve', $4500.00),
       (7, 3, 'Aaron', $5000.00),
       (8, 5, 'Lilach', $3500.00),
       (9, 7, 'Rita', $3000.00),
       (10, 5, 'Sean', $3000.00),
       (11, 7, 'Gabriel', $3000.00),
       (12, 9, 'Emilia', $2000.00),
       (13, 9, 'Michael', $2000.00),
       (14, 9, 'Didi', $1500.00);

create unique index idx_unc_mgrid_empid on dbo.Employees (mgrid, empid);

go
with EmpsRN as (
    select *,
           row_number() over (partition by mgrid order by empname, empid) as n
    from dbo.Employees
),
     EmpsPath as (
         select empid,
                empname,
                salary,
                0                          as lvl,
                cast(0x as varbinary(max)) as sortpath
         from dbo.Employees
         where mgrid is null

         union all

         select C.empid,
                C.empname,
                C.salary,
                P.lvl + 1,
                P.sortpath + cast(n as Binary(2)) as sortpath
         from EmpsPath as P
                  inner join EmpsRN as C on C.mgrid = P.empid
     )
select empid, salary, replicate('| ', lvl) + empname as empname
from EmpsPath
order by sortpath;