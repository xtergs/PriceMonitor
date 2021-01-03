import * as React from 'react'
import {User, UserManager} from "oidc-client";
import {Redirect, useLocation} from 'react-router-dom';
import {AppContext, GetManager} from "../../App";
import {ProgressIndicator} from "@fluentui/react";


export const SigninResponse = () => {

    const [processed, setProcessed] = React.useState(false)

    const manager = GetManager();

    const location = useLocation();
    console.log(location);

    const context = React.useContext(AppContext)

    manager.processSigninResponse(location.pathname + location.search).then(async u => {
        console.log(u);
        await manager.storeUser(new User(u as any))
        const user = await manager.getUser();
        console.log(user)
        if (!user){
            throw new Error("Failed to signin")
        }
        context.updateUser(user);
        setProcessed(true)
    });

    if (!processed){
        return (<ProgressIndicator />)
    }

    return <Redirect to={"/Success"} />;
}