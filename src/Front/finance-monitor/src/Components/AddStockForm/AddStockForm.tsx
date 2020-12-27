import * as React from "react";
import {Stock} from "../../Models/Stock";
import {Stack, TextField, DefaultButton} from 'office-ui-fabric-react';
import {useState} from "react";
import {Api} from "../../Api/Api";


interface IProps {
    onStockAdded: (stock: Stock) => void
}

export const AddStockForm = (props: IProps) => {

    const [isSubmitting, setSubmitting] = useState(false)
    const [symbol, setSymbol] = useState('');
    const [price, setPrice] = useState(0);
    const [count, setCount] = useState(1);

    const updateSymbol = (newValue:any)=>{
        setSymbol(newValue);
    }

    const updatePrice = (newValue:any)=>{
        setPrice(parseInt(newValue))
    }

    const updateCount = (newValue:any)=>{
        setCount(parseInt(newValue))
    }

    const add = async () => {
        if (isSubmitting)
            return;

        try {
            setSubmitting(true);

            const stock = await Api.addStock(symbol, price, count);
            props.onStockAdded(stock);
        } finally {
            setSubmitting(false)
        }
    }

    return (<Stack>
<TextField label={"Symbol"} onChanged={updateSymbol} />
<TextField label={"Price"} onChanged={updatePrice}/>
<TextField label={"Count"} onChanged={updateCount} />
<DefaultButton text={"Add"} disabled={isSubmitting} onClick={add}/>
    </Stack>)
}