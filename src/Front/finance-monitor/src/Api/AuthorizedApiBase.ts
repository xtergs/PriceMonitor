import {GetManager} from "../OidcConfig";

/**
 * Configuration class needed in base class.
 * The config is provided to the API client at initialization time.
 * API clients inherit from #AuthorizedApiBase and provide the config.
 */
export class IConfig {
    /**
     * Returns a valid value for the Authorization header.
     * Used to dynamically inject the current auth header.
     */
    //getAuthorization!: () => 'the-authentication-token';
}

export class AuthorizedApiBase {
    private readonly config: IConfig;

    protected constructor(config: IConfig) {
        this.config = config;
    }

    protected transformOptions = async (options: RequestInit): Promise<RequestInit> => {
        const user = await GetManager().getUser();
        if (user) {
            const token = `${user.token_type} ${user.access_token}`;
            options.headers = {
                ...options.headers,
                Authorization: token,
            };
        }
        return Promise.resolve(options);
    };
}