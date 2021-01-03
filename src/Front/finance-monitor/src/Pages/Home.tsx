import * as React from 'react'
import {DefaultButton, Stack} from "@fluentui/react";
import {AddStockForm} from "../Components/AddStockForm/AddStockForm";
import {UserManager} from "oidc-client";
import {GetManager} from "../App";
import {Authorize} from "./Authorize/Authorize";

export const Home = ()=>{


    const login = async () => {

        const manager = GetManager()

        await manager.signinRedirect()

        const user = await manager.getUser()
        console.log(user);
    }
    return (
        <Stack
            horizontalAlign="center"
            verticalAlign="center"
            verticalFill
            styles={{
                root: {
                    width: '960px',
                    margin: '0 auto',
                    textAlign: 'center',
                    color: '#605e5c'
                }
            }}
            gap={15}
        >
            <Authorize/>
            <AddStockForm onStockAdded={(stock)=> alert(JSON.stringify(stock))} />
            <img
                src="https://raw.githubusercontent.com/Microsoft/just/master/packages/just-stack-uifabric/template/src/components/fabric.png"
                alt="logo"
            />
        </Stack>
    )
}