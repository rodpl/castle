
# The filters added to this controller will be run for all controllers in the application.
# Likewise will all the methods added be available for all controllers.
class AbstractApplicationController < ApplicationController
  helper :application

  layout 'layouts/castlelayout'

end