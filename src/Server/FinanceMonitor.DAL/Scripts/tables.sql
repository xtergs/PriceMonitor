drop table if exists PriceDaily
drop table if exists PriceHistory
drop table if exists UserPrice
drop table if exists Stock
drop table if exists UserProfile
create table UserProfile
(
    Id nvarchar(512) not null primary key
)

go

if not exists(select *
              from sys.tables
              where name = 'Stock')
create table Stock
(
    Symbol            nvarchar(512) not null unique,
    Market            nvarchar(512) not null,
    Timezone          nvarchar(512),
    Time              datetime2,
    ShortName         nvarchar(512) not null,
    LongName          nvarchar(512),
    Currency          nvarchar(512) not null,
    FinancialCurrency nvarchar(512),
    Language          nvarchar(512) not null,
    LastClosed        datetime2     null,
    LastOpened        datetime2     null,
    QuoteType         nvarchar(512) not null,
    Status            nvarchar(50)  null

        constraint Stock_PK_Id Primary Key (Symbol),
)
if not exists(select *
              from sys.tables
              where name = 'UserPrice')
create table UserPrice
(
    Id          uniqueidentifier not null default NEWID(),
    UserId      nvarchar(512)    not null,
    StockSymbol nvarchar(512)    not null,

    Price       money            not null,
    Count       int              not null,
    DateTime    DATETIME2        not null,

    constraint UserPrice_PK_Id Primary Key (Id),

    constraint UserPrice_FK_StockId Foreign Key (StockSymbol) REFERENCES Stock (Symbol),
    constraint UserPrice_FK_UserId FOREIGN KEY (UserId) References UserProfile (Id)

)
if not exists(select *
              from sys.tables
              where name = 'PriceHistory')
create table PriceHistory
(
    Id          uniqueidentifier not null default NEWID(),
    StockSymbol nvarchar(512)    not null,
    Volume      float            not null,
    Opened      money            not null,
    Closed      money            not null,
    High        money            not null,
    Low         money            not null,
    DateTime    DATETIME2        not null,

    constraint PriceHistory_PK Primary Key (Id),
    constraint PriceHistory_FK Foreign Key (StockSymbol) references Stock (Symbol)
)
if not exists(select *
              from sys.tables
              where name = 'PriceDaily')
create table PriceDaily
(
    Id          uniqueidentifier not null default NEWID(),
    StockSymbol nvarchar(512)    not null,
    Ask         float,
    Bid         float,
    AskSize     float            not null default (0),
    BidSize     float            not null default (0),
    Time        datetime2        not null,
    Volume      float            not null,
    Price       float            not null,

    constraint PriceDaily_PK_Id Primary key (Id),
    constraint PriceDaily_FK_StockId Foreign Key (StockSymbol) references Stock (Symbol)

)

go

drop index if exists IX_DateTime_StockSymbol on PriceHistory
create unique index IX_DateTime_StockSymbol on PriceHistory (StockSymbol, DateTime)


----
go
drop table if exists SampledHistoryDataYearly
create table SampledHistoryDataYearly
(
    StockSymbol nvarchar(512) not null,
    DateTime    Date          not null,
    Volume      float         not null,
    Opened      money         not null,
    Closed      money         not null,
    High        money         not null,
    Low         money         not null,
    constraint SampledHistoryDataYearly_PK PRIMARY KEY (StockSymbol, DateTime)
)

--
drop index if exists idx_PriceDaily_Symbol_Time on PriceDaily
create index idx_PriceDaily_Symbol_Time on PriceDaily (StockSymbol, Time desc)
    include (Price, Volume);

--
go
if not exists(select *
              from sys.tables
              where name = 'StockSummary')
create table StockSummary
(
    Symbol        nvarchar(512) not null,
    CurrentTime   DATETIME2     not null,
    CurrentVolume float         not null,
    CurrentPrice  float         not null,

    constraint FK_StockSummary_Stock FOREIGN KEY (Symbol) references Stock (Symbol)
)
go
drop trigger if exists PriceDaily_INSERT
go
create trigger PriceDaily_INSERT
    on PriceDaily
    for insert
    as
begin
    declare @canUpdate int;
    set @canUpdate = (select count(*)
                      from StockSummary
                               join inserted on StockSymbol = Symbol
                      where CurrentTime < inserted.Time)
    if @canUpdate = 1
        begin
            Update StockSummary
            set CurrentTime   = Time,
                CurrentVolume = Volume,
                CurrentPrice  = Price
            from inserted
            where Symbol = StockSymbol
        end
    if @@ROWCOUNT = 0
        insert into StockSummary(Symbol, CurrentTime, CurrentVolume, CurrentPrice)
        select StockSymbol, Time, Volume, Price
        from inserted

end
go

--