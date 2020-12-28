import * as React from 'react'
import {UserManager} from "oidc-client";
import {useLocation} from 'react-router-dom';
import {GetManager} from "../../App";


export const SigninResponse = () => {

    const manager = GetManager();

    const location = useLocation();
    console.log(location);

    manager.processSigninResponse(location.pathname + location.search).then(u => console.log(u));

    return null;
}