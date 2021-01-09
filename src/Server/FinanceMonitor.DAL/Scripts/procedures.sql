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
drop procedure if exists GetStockDaily
go
create procedure GetStockDaily @Symbol nvarchar(512),
                               @Start datetime2,
                               @End datetime2
as
begin
    select PD.*
    from PriceDaily as PD
             join Stock S on PD.StockId = S.Id
    where Symbol = @Symbol
      and PD.Time between @Start and @End
    order by Time
end

go
drop procedure if exists GetUserStocks
go
create procedure GetUserStocks @UserId nvarchar(512)
as
begin
    select s.Id, s.Symbol, s.ShortName, s.LongName, SUM(UP.Count) as Shares, Sum(UP.Price * UP.Count) as Total
    from Stock as s
             inner join UserPrice UP on s.Id = UP.StockId
    where UP.UserId = @UserId
    group by s.Id, s.Symbol, s.ShortName, s.LongName
end


go
drop procedure if exists GetUserStockPrices
go
create procedure GetUserStockPrices @UserId nvarchar(512),
                                    @Symbol nvarchar(512)
as
begin
    select UP.*
    from UserPrice as UP
             join Stock S on UP.StockId = S.Id
    where UserId = @UserId
      and Symbol = @Symbol
end

go
drop procedure if exists GetStock
go
create procedure GetStock @Symbol nvarchar(512)
as
begin
    select *
    from Stock
    where Symbol = @Symbol
end

go
drop procedure if exists AddStock
go
create procedure AddStock @Symbol nvarchar(512),
                          @Market nvarchar(512),
                          @Timezone nvarchar(512),
                          @Time datetime2,
                          @ShortName nvarchar(512),
                          @LongName nvarchar(512),
                          @Currency nvarchar(512),
                          @FinancialCurrency nvarchar(512),
                          @Language nvarchar(512),
                          @QuoteType nvarchar(512)
as
begin
    Insert into Stock (Symbol, Market, Timezone, Time, ShortName, LongName, Currency, FinancialCurrency,
                       Language, QuoteType)
    Output Inserted.*
    values (@Symbol, @Market, @Timezone, @Time, @ShortName, @LongName, @Currency, @FinancialCurrency,
            @Language, @QuoteType)
end

go
drop procedure if exists AddUserPrice
go
create procedure AddUserPrice @UserId nvarchar(512),
                              @StockId uniqueidentifier,
                              @Price money,
                              @Count int,
                              @DateTime datetime2
as
begin
    Insert into UserPrice(UserId, StockId, Price, Count, DateTime)
    OUTPUT Inserted.*
    values (@UserId, @StockId, @Price, @Count, @DateTime)
end

go
drop procedure if exists AddDailyPrice
go
create procedure AddDailyPrice @StockId uniqueidentifier,
                               @Ask float,
                               @Bid float,
                               @AskSize float,
                               @BidSize float,
                               @Time datetime2,
                               @Price float,
                               @Volume float
as
begin
    Insert into PriceDaily (StockId, Ask, Bid, AskSize, BidSize, Time,
                            Price, Volume)
    values (@StockId, @Ask, @Bid, @AskSize, @BidSize, @Time, @Price, @Volume)
end

go
drop procedure if exists InsertHistory
go
create procedure InsertHistory @StockId uniqueidentifier,
                               @Volume float,
                               @Opened float,
                               @Closed float,
                               @High float,
                               @Low float,
                               @DateTime datetime2
as
begin
    Insert into PriceHistory (StockId, Volume, Opened, Closed, High, Low, DateTime)
    values (@StockId, @Volume, @Opened, @Closed, @High, @Low, @DateTime)
end

go
drop procedure if exists GetSavedStocks
go
create procedure GetSavedStocks
as
begin
    select S.*, PD.*
    from Stock as S

             join (select rank() over (partition by PD.StockId order by Time Desc) r,
                          PD.StockId,
                          PD.Price  as                                             CurrentPrice,
                          PD.Time   as                                             CurrentTime,
                          PD.Volume as                                             CurrentVolume
                   from PriceDaily as PD
    ) as PD on S.Id = PD.StockId and PD.r = 1

end
go
drop procedure if exists GetStockHistoryYearly
go
create procedure GetStockHistoryYearly @Symbol nvarchar(512),
                                       @Start datetime2,
                                       @End datetime2
as
begin
    select *
    from (select Rank() over (partition by StockId, Year(DateTime) order by DateTime desc) as r, PH.*
          from PriceHistory as PH
                   join Stock on PH.StockId = Stock.Id
          where Stock.Symbol = @Symbol
         ) as PH
    where PH.r = 1 and DateTime between @End and @Start
    order by DateTime asc
end

go
drop procedure if exists GetStockHistoryMonthly
go
create procedure GetStockHistoryMonthly @Symbol nvarchar(512),
                                        @Start datetime2,
                                        @End datetime2
as
begin
    select *
    from (select Rank() over (partition by Year(DateTime), Month(DateTime) order by DateTime desc) as r, PH.*
          from PriceHistory as PH
                   join Stock on PH.StockId = Stock.Id
          where Stock.Symbol = @Symbol
         ) as PH
    where PH.r = 1 and DateTime between @End and @Start
    order by DateTime asc
end

go
drop procedure if exists GetStockHistoryDaily
go
create procedure GetStockHistoryDaily @Symbol nvarchar(512),
                                      @Start datetime2,
                                      @End datetime2
as
begin
    select PH.*
    from PriceHistory as PH
             join Stock on PH.StockId = Stock.Id
    where Stock.Symbol = @Symbol
      and DateTime between @End and @Start
    order by DateTime asc
end

go
drop procedure if exists GetStockHistory
go
create procedure GetStockHistory @Symbol nvarchar(512),
                                 @Type int = 0,
                                 @Start datetime2 = null,
                                 @End datetime2 = null
as
begin 
    if @Type = 0
        exec dbo.GetStockHistoryDaily @Symbol, @Start, @End
    else
        if @Type = 1
            exec dbo.GetStockHistoryMonthly @Symbol, @Start, @End
        else
            exec dbo.GetStockHistoryYearly @Symbol, @Start, @End
end