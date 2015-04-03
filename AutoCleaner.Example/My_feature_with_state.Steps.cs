using System;
using AutoCleaner.Example.Domain;
using LightBDD;
using NUnit.Framework;

namespace AutoCleaner.Example
{
    public partial class My_feature_with_state : FeatureFixture
    {
        private readonly Database _db = new Database();
        private string _name;
        private string _login;
        private Guid _id;
        private Exception _exception;

        /// <summary>
        /// Normally it would be necesarry to reset all fields to default values, but StateCleaner can do it automatically.
        /// Please note that HierarchyOptions.Declared guarantees that state belonging to FeatureFixture base class will remain inact, so Runner would not be reset.
        /// Also, by default, StateCleaner does not clean readonly fields, so _db field would remain inact
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            StateCleaner.ResetInstance(this, HierarchyOptions.Declared);
        }

        private void A_new_user_with_name(string name)
        {
            _name = name;
        }

        private void User_login_is_specified(string login)
        {
            _login = login;
        }

        private void User_click_add_button()
        {
            try
            {
                _id = _db.AddUser(_name, _login);
            }
            catch (Exception e)
            {
                _exception = e;
            }
        }

        private void User_is_added_to_database()
        {
            Assert.That(_exception, Is.Null);
        }

        private void User_identifier_is_returned()
        {
            Assert.That(_id, Is.Not.EqualTo(Guid.Empty));
        }

        private void It_is_possible_to_retrieve_user_details_with_received_identifier()
        {
            Assert.That(_db.GetUser(_id), Is.Not.Null);
        }

        private void It_is_possible_to_retrieve_user_details_with_login()
        {
            Assert.That(_db.FindUserByName(_name), Is.Not.Null);
        }

        private void An_error_is_displayed(string error)
        {
            Assert.That(_exception, Is.Not.Null);
            Assert.That(_exception.Message, Is.EqualTo(error));
        }

        private void User_is_not_added_to_database()
        {
            Assert.That(_db.FindUserByName(_name), Is.Null);
        }
    }
}