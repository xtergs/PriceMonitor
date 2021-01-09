import * as React from "react";
import {useState} from "react";
import {DatePicker, DefaultButton, Stack, TextField} from '@fluentui/react';
import {UserPrice, UserPricingClient} from "../../Api/ApiClients";
import {host} from "../../Api/Consts";


interface IProps {
    onStockAdded: (stock: UserPrice) => void
}

export const AddStockForm = (props: IProps) => {

    const [isSubmitting, setSubmitting] = useState(false)
    const [symbol, setSymbol] = useState('');
    const [price, setPrice] = useState(0);
    const [count, setCount] = useState(1);
    const [date, setDate] = useState(new Date());

    const updateSymbol = (e: any, newValue: any) => {
        setSymbol(newValue);
    }

    const updatePrice = (e: any,newValue: any) => {
        setPrice(parseInt(newValue))
    }

    const updateCount = (e: any,newValue: any) => {
        setCount(parseInt(newValue))
    }

    const updateDate = (newDate?: Date | null) => {
        if (!newDate)
            return;
        setDate(newDate)
    }

    const add = async () => {
        if (isSubmitting)
            return;

        try {
            setSubmitting(true);

            const stock = await new UserPricingClient({}, host).userPricing({
                count: count,
                dateTime: date,
                price: price,
                symbol: symbol,
                userId: ''
            });
            props.onStockAdded(stock);
        } finally {
            setSubmitting(false)
        }
    }

    return (<Stack>
        <TextField label={"Symbol"} onChange={updateSymbol}/>
        <TextField label={"Price"} onChange={updatePrice}/>
        <TextField label={"Count"} onChange={updateCount}/>
        <DatePicker label={"Date"} onSelectDate={updateDate}/>
        <DefaultButton text={"Add"} disabled={isSubmitting} onClick={add}/>
    </Stack>)
}