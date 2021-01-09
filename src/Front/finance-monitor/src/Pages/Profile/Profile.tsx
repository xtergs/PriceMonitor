import * as React from 'react'
import {useEffect, useMemo} from 'react'
import {useHistory} from "react-router-dom";
import {UserPrice, UserPricingClient, UserStock} from "../../Api/ApiClients";
import {host} from "../../Api/Consts";
import {CommandBar, DetailsList, Dialog, DialogType, ICommandBarItemProps, ProgressIndicator, SelectionMode} from "@fluentui/react";
import {AddStockForm} from "../../Components/AddStockForm/AddStockForm";

interface IProps {

}

export const Profile = (props: IProps) => {

    const history = useHistory();

    const [isLoading, setLoading] = React.useState(true);
    const [isShowAddDialog, setShowAddDialog] = React.useState(false)
    const [stocks, setStocks] = React.useState<UserStock[]>([])

    const closeAddDialog = () => setShowAddDialog(false)

    const commands = useMemo((): ICommandBarItemProps[]=>{
        return [{
            key: "add",
            text: "New",
            iconProps: { iconName: 'Add' },
            onClick: ()=>{
                setShowAddDialog(true)
            }
        }]
    }, [])

    const refresh = ()=>{
        setLoading(true)
        return new UserPricingClient({}, host).list()
            .then(stocks => {
                setStocks(stocks);
            })
            .finally(() => setLoading(false))
    }

    useEffect(() => {
        refresh()
    }, [])



    if (isLoading) {
        return <ProgressIndicator label={"Loading"}/>
    }

    return (<><h1>Profile</h1>
        <CommandBar items={commands}
        />
        <DetailsList items={stocks} selectionMode={SelectionMode.none}
                     compact={true}
                     onActiveItemChanged={(item: UserStock, index, ev) => {
                         if (!item)
                             return;
                         history.push(`${history.location.pathname}/${item.symbol}`)

                     }}
        />
        <Dialog type={DialogType.close}
                hidden={!isShowAddDialog}
                onDismissed={closeAddDialog}
                modalProps={{
                    isBlocking: true,

                }}>
            <AddStockForm onStockAdded={(stock) => {
                refresh();
                closeAddDialog();
            }}/>
        </Dialog>
    </>)
}