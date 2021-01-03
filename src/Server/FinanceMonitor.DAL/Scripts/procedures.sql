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