-- drop table if exists UserPrice
-- drop table if exists Stock
-- drop table if exists PriceHistory
-- drop table if exists PriceDaily
if
not exists (select * from sys.tables where name = 'Stock')

create table Stock
(
    Id                uniqueidentifier not null default NEWId(),
    Symbol            nvarchar(512) not null unique,
    Market            nvarchar(512) not null,
    Timezone          nvarchar(512),
    Time              datetime2,
    ShortName         nvarchar(512) not null,
    LongName          nvarchar(512) not null,
    Currency          nvarchar(512) not null,
    FinancialCurrency nvarchar(512) not null,
    Language          nvarchar(512) not null,
    LastClosed        datetime2 null,
    LastOpened        datetime2 null,

    constraint Stock_PK_Id Primary Key (Id),
) if not exists(select * from sys.tables where name = 'UserPrice')
create table UserPrice
(
    Id       uniqueidentifier not null default NEWID(),
    UserId   uniqueidentifier not null,
    StockId  uniqueidentifier not null,

    Price    money            not null,
    Count    int              not null,
    DateTime DATETIME2        not null,

    constraint UserPrice_PK_Id Primary Key (Id),

    constraint UserPrice_FK_StockId Foreign Key (StockId) REFERENCES Stock (Id)

) if not exists (select * from sys.tables where name = 'PriceHistory')
create table PriceHistory
(
    Id       uniqueidentifier not null default NEWID(),
    StockId  uniqueidentifier not null,
    Volume   float            not null,
    Opened   money            not null,
    Closed   money            not null,
    High     money            not null,
    Low      money            not null,
    DateTime DATETIME2        not null,

    constraint PriceHistory_PK Primary Key (Id),
    constraint PriceHistory_FK Foreign Key (StockId) references Stock (Id)
) if not exists (select * from sys.tables where name = 'PriceDaily')
create table PriceDaily
(
    Id      uniqueidentifier not null default NEWID(),
    StockId uniqueidentifier not null,
    Ask     float            not null,
    Bid     float            not null,
    AskSize float            not null,
    BidSize float            not null,
    Time    datetime2        not null,

    constraint PriceDaily_PK_Id Primary key (Id),
    constraint PriceDaily_FK_StockId Foreign Key (StockId) references Stock (Id)

)