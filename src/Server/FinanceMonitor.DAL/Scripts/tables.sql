-- drop table if exists PriceDaily
-- drop table if exists PriceHistory
-- drop table if exists UserPrice
-- drop table if exists Stock
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
drop table if exists UserProfile
create table UserProfile
(
    Id nvarchar(512) not null primary key
)

go

drop index if exists IX_DateTime_StockSymbol on PriceHistory
create unique index IX_DateTime_StockSymbol on PriceHistory (StockSymbol, DateTime)