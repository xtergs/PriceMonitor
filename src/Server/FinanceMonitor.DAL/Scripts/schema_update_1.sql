go

alter table PriceHistory
    Add StockSymbol nvarchar(512);

alter table PriceDaily
    Add StockSymbol nvarchar(512);

alter table UserPrice
    Add StockSymbol nvarchar(512);
go
begin transaction


alter table PriceHistory
    drop constraint PriceHistory_FK;

update PriceHistory
set PriceHistory.StockSymbol = ST.Symbol
from Stock as ST
where ST.Id = PriceHistory.StockId

alter table PriceHistory
    add constraint PriceHistory_FK_Symbol Foreign Key (StockSymbol) references Stock(Symbol)


alter table PriceDaily
    drop constraint PriceDaily_FK_StockId


update PriceDaily
set PriceDaily.StockSymbol = ST.Symbol
from Stock as ST
where ST.Id = PriceDaily.StockId

alter table PriceDaily
    add constraint PriceDaily_FK_Symbol Foreign Key (StockSymbol) references Stock(Symbol)


alter table UserPrice
    drop constraint UserPrice_FK_StockId

update UserPrice
set UserPrice.StockSymbol = ST.Symbol
from Stock as ST
where ST.Id = UserPrice.StockId

alter table UserPrice
    add constraint UserPrice_FK_Symbol Foreign Key (StockSymbol) references Stock(Symbol)

alter table Stock
    drop constraint Stock_PK_Id

commit transaction
go

alter table Stock
    add constraint Stock_PK_Symbol Primary Key (Symbol)

go

alter table PriceDaily
    drop column StockId
alter table PriceHistory
    drop column StockId
alter table UserPrice
    drop column StockId
go
alter table Stock
drop constraint DF__Stock__Id__762C88DA
alter table Stock
    drop column Id

go

alter table PriceHistory
alter COLUMN DateTime Date
