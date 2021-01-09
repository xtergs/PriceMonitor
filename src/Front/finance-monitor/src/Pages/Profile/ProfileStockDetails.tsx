import * as React from "react";
import {useEffect, useState} from "react";
import {useParams} from "react-router-dom";
import {UserPrice, UserPricingClient} from "../../Api/ApiClients";
import {host} from "../../Api/Consts";
import {DetailsList, ProgressIndicator, SelectionMode, Stack} from "@fluentui/react";

interface IProps{

}

export const ProfileStockDetails = (props: IProps)=>{
    const [loading, setLoading] = useState(true)
    const {symbol} = useParams<{ symbol: string }>()

    const [shares, setShares] = useState<UserPrice[]>([])

    useEffect(() => {
        new UserPricingClient({}, host).shares(symbol)
            .then(x => {
                setShares(x);
            })
            .finally(() => setLoading(false))
    },[])

    if (loading) {
        return <ProgressIndicator label={"Loading"}/>
    }

    return (<>
        <Stack>
        <h1></h1>
            <DetailsList
            items={shares}
            compact={true}
            selectionMode={SelectionMode.none}
            >

            </DetailsList>
        </Stack>
    </>)
}