import * as React from 'react'
import {PrimaryButton, Separator, Stack, Text} from "@fluentui/react";
import {Register} from "../Register/Register";
import {GetManager} from "../../App";

interface IProps {

}

export const Authorize = (props: IProps) => {

    const login = async () => {

        const manager = GetManager()

        await manager.signinRedirect()

        const user = await manager.getUser()
        console.log(user);
    }

    return (<>
        <Stack>
            <Text>Welcome to PriceMonitor app</Text>
            <Text>Register</Text>
            <Register/>
            <Separator title={"OR"}/>
            <PrimaryButton onClick={login}>Sign In</PrimaryButton>
        </Stack>
    </>)
}