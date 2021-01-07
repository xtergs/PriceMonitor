drop procedure if exists AddUser
go
create procedure AddUser @Id nvarchar(512)
as
begin
    if not exists(select Id
                  from UserProfile
                  where Id = @Id)
        begin
            insert into UserProfile
            values (@Id)
        end
end
go
drop procedure if exists GetStock
go
create procedure GetStock @Symbol nvarchar(512),
@Time datetime2
as
    begin
        select PH.* from PriceHistory as PH
        join Stock on PH.StockId = Stock.Id
        where Stock.Symbol = @Symbol and DateTime > @Time
        order by DateTime
    end

go
drop procedure if exists GetStockDaily
go
create procedure GetStockDaily @Symbol nvarchar(512),
@Start datetime2,
@End datetime2
as
    begin
        select PD.* from PriceDaily as PD
        join Stock S on PD.StockId = S.Id
        where Symbol = @Symbol and
              PD.Time between @Start and @End
        order by Time
    end