import "../../pages/home/styles/HomePage.css";
import StatsPartial from "../../partials/stats/StatsPartial";
import { UserContext } from "../../contexts/UserContext";
import { useContext } from "react";

//Shows stats partial if user is logged in
const HomePage = () =>{
    const {userUpdate, user} = useContext(UserContext);

    return(
        <div>
            {
                user == null || user.jwt === "" ?
                <>
                    <p>Please login to view your game stats.</p>
                </>
            : 
                <>
                    <StatsPartial jwt={user.jwt}></StatsPartial>
                </>
            }
    </div>
    );
}

export default HomePage;