using LightBDD;
using NUnit.Framework;

namespace AutoCleaner.Example
{
    [FeatureDescription(
@"In order to focus on meaningful code
As a devloper
I want to eliminate any boilerplate and run my tests in isolation")]
    [TestFixture]
    public partial class My_feature_with_state
    {
        [Test]
        public void Adding_new_user_to_database()
        {
            Runner.RunScenario(
                given => A_new_user_with_name("Tom"),
                and => User_login_is_specified("tomxx2"),
                when => User_click_add_button(),
                then => User_is_added_to_database(),
                and => User_identifier_is_returned(),
                and => It_is_possible_to_retrieve_user_details_with_received_identifier(),
                and => It_is_possible_to_retrieve_user_details_with_login());
        }

        [Test]
        public void Mandatory_fields_validation()
        {
            Runner.RunScenario(
                given => A_new_user_with_name("Laura"),
                when => User_click_add_button(),
                then => An_error_is_displayed("Please provide login"),
                and => User_is_not_added_to_database());
        }
    }
}