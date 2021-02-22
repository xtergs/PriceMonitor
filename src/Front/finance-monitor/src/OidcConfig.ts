import {UserManager} from "oidc-client";
import {identityHost} from "./Api/Consts";

const manager = new UserManager({
    client_id: "interactive",
    client_secret: "49C1A7E1-0C79-4A89-A3D6-A37998FB86B0",
    scope: "openid profile scope2",
    authority: identityHost,
    redirect_uri: "http://localhost:3000/signin",
    response_type: "code",
    post_logout_redirect_uri: "http://localhost:3000/logout-callback"

});

export const GetManager = () => manager;