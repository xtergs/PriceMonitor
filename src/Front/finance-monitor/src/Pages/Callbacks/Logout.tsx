import * as React from 'react'
import {AppContext, GetManager} from "../../App";
import {Redirect, useLocation} from "react-router-dom";
import {useContext} from "react";

export const Logout = ()=>{

    const location = useLocation();
    console.log(location);

    const context = useContext(AppContext);
        
    const manage = GetManager();
    manage.processSignoutResponse(location.pathname + location.search).then(x=>{
        console.log(x);
    })
    context.updateUser(null)
    return (<Redirect to={"/"} />);
}